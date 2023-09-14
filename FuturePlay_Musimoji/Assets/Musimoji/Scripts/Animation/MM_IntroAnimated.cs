using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Musimoji.Scripts.Animation
{
	public class MM_IntroAnimated : MM_Intro
	{
		[SerializeField] private GameObject introObject;
		[SerializeField] private Animator animator;
		[SerializeField] private AnimationClip introClip;

		protected override void Update()
		{
			
		}
		
		public override void StartIntro()
		{
			if(DebugMessages) Debug.Log("MM_IntroAnimated.StartIntro");
			introActive = true;
			animator.SetTrigger("Play");
			introCanvas.gameObject.SetActive(true);
			StartCoroutine(TimedIntro(introClip.length));
		}

		protected override void SlideShowComplete()
		{
			if(DebugMessages) Debug.Log("MM_IntroAnimated.SlideShowComplete");
			introActive = false;
			introTimer = 0;
			introObject.SetActive(false);
			OnStartGame?.Invoke();
		}

		private IEnumerator TimedIntro(float duration)
		{
			yield return new WaitForSeconds(duration);
			SlideShowComplete();
		}
	}
}