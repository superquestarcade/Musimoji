/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System.Text;
using UnityEngine;
using System.Threading;

/**
 * While 'SerialController' only allows reading/sending text data that is
 * terminated by new-lines, this class allows reading/sending messages 
 * using a binary protocol where each message is separated from the next by 
 * a 1-char delimiter.
 */
public class ArduinoLEDControl : MonoBehaviour
{
    [Tooltip("Port name with which the SerialPort object will be created.")]
    public string portName = "COM3";

    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int baudRate = 9600;

    [Tooltip("Reference to an scene object that will receive the events of connection, " +
             "disconnection and the messages from the serial device.")]
    public GameObject messageListener;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int reconnectionDelay = 1000;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public int maxUnreadMessages = 1;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public byte separator = 90;

    // Internal reference to the Thread and the object that runs in it.
    protected Thread thread;
    protected static SerialThreadBinaryDelimited serialThread;

    //Singleton code
    //private static GameCabinet aCabinet = 0;
    //private static LEDPlayState currentGameState = LEDPlayState.STANDBY;

    public static ArduinoLEDControl Singleton { get; private set; }
    public bool dontDestroyOnLoad = true;
    private void Awake()
    {
        InitializeSingleton();
    }
    private void Start()
    {
        SetState(LEDPlayState.ATTRACT);
    }


    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is activated.
    // It creates a new thread that tries to connect to the serial device
    // and start reading from it.
    // ------------------------------------------------------------------------
    private void OnEnable()
    {
        serialThread = new SerialThreadBinaryDelimited(portName,
                                                       baudRate,
                                                       reconnectionDelay,
                                                       maxUnreadMessages,
                                                       separator);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
    }

    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is deactivated.
    // It stops and destroys the thread that was reading from the serial device.
    // ------------------------------------------------------------------------
    private void OnDisable()
    {
        SetState(LEDPlayState.STANDBY);
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        if (userDefinedTearDownFunction != null)
            userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    // ------------------------------------------------------------------------
    // Polls messages from the queue that the SerialThread object keeps. Once a
    // message has been polled it is removed from the queue. There are some
    // special messages that mark the start/end of the communication with the
    // device.
    // ------------------------------------------------------------------------
    private void Update()
    {
        // If the user prefers to poll the messages instead of receiving them
        // via SendMessage, then the message listener should be null.
        if (messageListener == null)
            return;

        // Read the next message from the queue
        byte[] message = ReadSerialMessage();
        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        messageListener.SendMessage("OnMessageArrived", message);
    }

    // ------------------------------------------------------------------------
    // Returns a new unread message from the serial device. You only need to
    // call this if you don't provide a message listener.
    // ------------------------------------------------------------------------
    public byte[] ReadSerialMessage()
    {
        // Read the next message from the queue
        return (byte[])serialThread.ReadMessage();
    }

    // ------------------------------------------------------------------------
    // Puts a message in the outgoing queue. The thread object will send the
    // message to the serial device when it considers it's appropriate.
    // ------------------------------------------------------------------------
    public static void SendSerialMessage(byte[] message)
    {
        Debug.Log("Send message called.");
        serialThread.SendMessage(message);
    }

    // ------------------------------------------------------------------------
    // Executes a user-defined function before Unity closes the COM port, so
    // the user can send some tear-down message to the hardware reliably.
    // ------------------------------------------------------------------------
    public delegate void TearDownFunction();
    private TearDownFunction userDefinedTearDownFunction;
    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this.userDefinedTearDownFunction = userFunction;
    }



    //static method for changing / setting state for games to call. 
    public static void SetState(LEDPlayState gameState)
    {
        //currentGameState = gameState;
        var sb = (byte) gameState;
        Debug.Log("changing state to " + gameState + " " + (char)sb);
        if ((char)sb != '-')
            SendSerialMessage(new byte[] { sb, 32 });
    }
    
    public static LEDPlayState[] GetAllPlayStates = 
    {
        LEDPlayState.STATIC_YELLOW,
        LEDPlayState.STATIC_GREEN,
        LEDPlayState.STATIC_TEAL,
        LEDPlayState.STATIC_LIGHTBLUE,
        LEDPlayState.STATIC_BLUE,
        LEDPlayState.STATIC_VIOLET,
        LEDPlayState.STATIC_RED,
        LEDPlayState.STATIC_ORANGE,
        LEDPlayState.BLINKING_YELLOW,
        LEDPlayState.BLINKING_GREEN,
        LEDPlayState.BLINKING_TEAL,
        LEDPlayState.BLINKING_LIGHTBLUE,
        LEDPlayState.BLINKING_BLUE,
        LEDPlayState.BLINKING_VIOLET,
        LEDPlayState.BLINKING_RED,
        LEDPlayState.BLINKING_ORANGE,
        LEDPlayState.BREATHING_YELLOW,
        LEDPlayState.BREATHING_GREEN,
        LEDPlayState.BREATHING_TEAL,
        LEDPlayState.BREATHING_LIGHTBLUE,
        LEDPlayState.BREATHING_BLUE,
        LEDPlayState.BREATHING_VIOLET,
        LEDPlayState.BREATHING_RED,
        LEDPlayState.BREATHING_ORANGE,
        LEDPlayState.STANDBY,
        LEDPlayState.ATTRACT,
        LEDPlayState.CHAOS,
        LEDPlayState.PLAYING
    };

    public static LEDPlayState GetStaticStateFromEmoji(int emojiIndex)
    {
        // emojiIndex = 0 is empty/ null
        if (emojiIndex is < 1 or > 8) return LEDPlayState.NULL;
        // Static colours:
        // STATIC_YELLOW = 49,
        // STATIC_GREEN = 50,
        // STATIC_TEAL = 51,
        // STATIC_LIGHTBLUE = 52,
        // STATIC_BLUE = 53,
        // STATIC_VIOLET = 54,
        // STATIC_RED = 55,
        // STATIC_ORANGE = 56,
        return (LEDPlayState) 48 + emojiIndex;
    }
    
    public static LEDPlayState GetBlinkingStateFromEmoji(int emojiIndex)
    {
        // emojiIndex = 0 is empty/ null
        if (emojiIndex is < 1 or > 8) return LEDPlayState.NULL;
        // Blinking colours:
        // BLINKING_YELLOW = 97,
        // BLINKING_GREEN = 98,
        // BLINKING_TEAL = 99,
        // BLINKING_LIGHTBLUE = 100,
        // BLINKING_BLUE = 101,
        // BLINKING_VIOLET = 102,
        // BLINKING_RED = 103,
        // BLINKING_ORANGE = 104,
        return (LEDPlayState) 96 + emojiIndex;
    }
    
    public static LEDPlayState GetBreathingStateFromEmoji(int emojiIndex)
    {
        // emojiIndex = 0 is empty/ null
        if (emojiIndex is < 1 or > 8) return LEDPlayState.NULL;
        // Breathing colours:
        // BREATHING_YELLOW = 105,
        // BREATHING_GREEN = 106,
        // BREATHING_TEAL = 107,
        // BREATHING_LIGHTBLUE = 108,
        // BREATHING_BLUE = 109,
        // BREATHING_VIOLET = 110,
        // BREATHING_RED = 111,
        // BREATHING_ORANGE = 112,
        return (LEDPlayState) 104 + emojiIndex;
    }

    private bool InitializeSingleton()
    {
        if (Singleton != null && Singleton == this) return true;

        if (dontDestroyOnLoad)
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Multiple singletons detected in the scene. Only one UIManager can exist at a time. The duplicate UIManager will be destroyed.");
                Destroy(gameObject);
                return false;
            }
            Debug.Log("created singleton (DontDestroyOnLoad)");
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("created singleton (ForScene)");
            Singleton = this;
        }

        return true;
    }
}
public enum LEDPlayState
{
    NULL = 0,
    // Static colours:
    STATIC_YELLOW = 49,
    STATIC_GREEN = 50,
    STATIC_TEAL = 51,
    STATIC_LIGHTBLUE = 52,
    STATIC_BLUE = 53,
    STATIC_VIOLET = 54,
    STATIC_RED = 55,
    STATIC_ORANGE = 56,
    
    // Blinking colours:
    BLINKING_YELLOW = 97,
    BLINKING_GREEN = 98,
    BLINKING_TEAL = 99,
    BLINKING_LIGHTBLUE = 100,
    BLINKING_BLUE = 101,
    BLINKING_VIOLET = 102,
    BLINKING_RED = 103,
    BLINKING_ORANGE = 104,
    
    // Breathing colours:
    BREATHING_YELLOW = 105,
    BREATHING_GREEN = 106,
    BREATHING_TEAL = 107,
    BREATHING_LIGHTBLUE = 108,
    BREATHING_BLUE = 109,
    BREATHING_VIOLET = 110,
    BREATHING_RED = 111,
    BREATHING_ORANGE = 112,
    
    STANDBY = 119,
    ATTRACT = 120,
    CHAOS = 121,
    PLAYING = 122,
}
