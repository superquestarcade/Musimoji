#include <FastLED.h>

#define DATA_PIN 2

#define LED_TYPE    WS2812B
#define COLOR_ORDER GRB
#define BRIGHTNESS         200
#define FRAMES_PER_SECOND  60

#define NUM_LEDS  79
CRGB leds[NUM_LEDS];

int colour = HUE_PINK;
int ledParams [] = {colour, 255, 255};

int inByte = 0;
int currentStateNum = 0;

uint8_t gHue = 0;


void setup() {
  delay(3000); // sanity delay
 
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
  FastLED.clear();
  FastLED.setBrightness( BRIGHTNESS );
  Serial.begin(9600);
  setLeds(ledParams);
}

void setLeds(int params [3]){
  for(int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CHSV(params[0], 255, 255);
  }
}

void loop()
{
   
   if (Serial.available() > 0) {
    // get incoming byte:
      inByte = Serial.read();
    Serial.println(inByte);
   }
   InputCheck(inByte);

  //could have it so game can send a special sequence in via serial
  //and when recieved it will interupt current state sequence, play fully, then continue
   RunGameState();

   FastLED.show(); // display this frame
   FastLED.delay(1000 / FRAMES_PER_SECOND);
   //call state func
}

//if its not a state change, its a cabniet setup
void InputCheck(byte input) {
  switch (input) {
      case 'w':    
        currentStateNum = 0;
        break;
      case 'x':    
        currentStateNum = 1;
        break;
      case 'y':
        currentStateNum = 2;
      break;
      case 'z':   
        currentStateNum = 3;
      break;
  }
}


void RunGameState() {
  if (currentStateNum == 0) {
    Standby();
  }
  else if (currentStateNum == 1) {
    Attract();
  }
  else if (currentStateNum == 2) {
    Play();
  }
  else if (currentStateNum == 3) {
    Win();
  }
  //here if an interupt sequence is recieved, play that instead i guess
  
}

void Standby() {
  ledParams[0] = HUE_BLUE;
  setLeds(ledParams);
}

void Attract() {
  rainbow();
  blendBaseHue();
}

void Play() {
  confetti();
}

void Win() {
  juggle();
}



//sequences 

void BlinkBlocks() {
  for(int i = 0; i < 20; i++) {
    leds[i] = CHSV(HUE_PINK, 255, 255);
  }
  for(int i = 20; i < 40; i++) {
    leds[i] = CHSV(HUE_BLUE, 255, 255);
  }
  for(int i = 40; i < 60; i++) {
    leds[i] = CHSV(HUE_RED, 255, 255);
  }
  for(int i = 60; i < 80; i++) {
    leds[i] = CHSV(HUE_GREEN, 255, 255);
  }
}

void blendBaseHue(){
    EVERY_N_MILLISECONDS(50) {gHue++;}
}

void confetti() {
  fadeToBlackBy( leds, NUM_LEDS, 20);
  int pos = random16(NUM_LEDS);
  leds[pos] += CHSV( gHue + random8(64), 200, 255);
}

void bpm() {
  uint8_t BeatsPerMinute = 62;
  CRGBPalette16 palette = PartyColors_p;
  uint8_t beat = beatsin8(BeatsPerMinute, 64, 255);
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = ColorFromPalette(palette, gHue+(i*2), beat-gHue+(i*10));
  }
}

void juggle() {
  fadeToBlackBy( leds, NUM_LEDS, 20);
  byte dothue = 0;
  for( int i = 0; i < 8; i++) {
    leds[beatsin16( i+7, 0, NUM_LEDS-1 )] |= CHSV(dothue, 200, 255);
    dothue += 32;
  }
}

void rainbow() {
  fill_rainbow(leds, NUM_LEDS, gHue, 7);
}

void addGlitter(fract8 chanceOfGlitter){
  if (random8() < chanceOfGlitter) {
    leds[random16(NUM_LEDS)] += CRGB::White;
  }
}

void rainbowWithGlitter() {
  rainbow(); 
  addGlitter(80);
}

void sinelon() {
  //loop 
  fadeToBlackBy(leds, NUM_LEDS, 20);
  int pos = beatsin16(13,0,NUM_LEDS-1);
  leds[pos] += CHSV(gHue, 255,192);
}
