﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Game;

namespace LightItUp.UI
{
	public class UI_Game : MonoBehaviour
	{
		public GameObject tutorialOverlay;
		public GameObject leftHand;
		public GameObject rightHand;
		public GameObject middleLine;
		public GameObject middleLineBottom;
		public List<Animator> anims;
		public GameObject levelName;
		public GameObject levelNameBackdrop;
		//public TextMeshProUGUI feedbackPoints;
		public GameObject progress;

		public Button pauseButton;

		void Awake()
		{
			PlayerController.StateChangedEvent += PlayerControllerOnStateChangedEvent;
		}



		void OnDestroy()
		{
			PlayerController.StateChangedEvent -= PlayerControllerOnStateChangedEvent;
		}

		private void PlayerControllerOnStateChangedEvent(PlayerController.State state)
		{
			pauseButton.interactable = (state == PlayerController.State.Normal);
		}

		private void OnEnable()
		{
			CanvasController.TogglePresenter (false);
			if (IphoneXSupport.IsPhoneX())
			{
				var scale = rightHand.transform.localScale;

				scale.x = 0.9f;
				scale.y = 0.9f;

				rightHand.transform.localScale = scale;
			
				scale = leftHand.transform.localScale;

				scale.x = -0.9f;
				scale.y = 0.9f;

				leftHand.transform.localScale = scale;


				var pos = progress.GetComponent<RectTransform>().anchoredPosition;

				pos.x = 100f;

				progress.GetComponent<RectTransform>().anchoredPosition = pos;
			} 
			
			tutorialOverlay.SetActive(GameData.PlayerData.showControlsTutorial || GameData.PlayerData.selectedLevelIdx == 0);
			rightHand.SetActive(true);
			leftHand.SetActive(true);
			rightHand.GetComponent<Animator>().SetTrigger("AnimateIn");
			leftHand.GetComponent<Animator>().SetTrigger("AnimateInDelayed");
			middleLine.SetActive(true);
			middleLineBottom.SetActive(true);
			GameManager.Instance.playerStart += OnPlayerStart;
			//startHand.SetActive(true);
			levelName.SetActive(true);
			levelNameBackdrop.SetActive(true);
			rightHand.GetComponent<Animator>().SetBool("ShowGameTutorial", GameData.PlayerData.showControlsTutorial);
			leftHand.GetComponent<Animator>().SetBool("ShowGameTutorial", GameData.PlayerData.showControlsTutorial);
			AdController.SetBannerAdArea(GetComponent<RectTransform>());


		}

		private void OnPlayerStart()
        {
			ShowUI();
			HideHand();
        }

		public void DebugLightAllBlocks()
		{
			if (Debug.isDebugBuild)
			{
				foreach (var blockController in FindObjectsOfType<BlockController>())
				{
					blockController.Collide(false);
				}
			}		
		}
	
		public void ShowUI()
		{
			foreach (var a in anims)
			{
				a.gameObject.SetActive(true);
				a.Play("ScaleIn");
			}
			levelName.GetComponent<Animator>().Play("ScaleOut");
			levelNameBackdrop.GetComponent<Animator>().Play("FadeOut");
		}
		public void HideHand()
		{
			rightHand.GetComponent<Animator>().SetTrigger("GameStart");
			leftHand.GetComponent<Animator>().SetTrigger("GameStart");
			middleLine.GetComponent<Animator>().Play("FadeOut");
			middleLineBottom.GetComponent<Animator>().Play("FadeOut");
		}

		public void ShowHand()
		{						
			rightHand.GetComponent<Animator>().SetTrigger("Preload");
			leftHand.GetComponent<Animator>().SetTrigger("Preload");

			LeanTween.delayedCall(0.1f, () =>
			{				
				rightHand.GetComponent<Animator>().SetTrigger("AnimateIn");
				leftHand.GetComponent<Animator>().SetTrigger("AnimateInDelayed");
			}).setIgnoreTimeScale(true);
		}
		
		public void Hide()
		{
			foreach (var a in anims)
			{
				a.Play("ScaleOut");
				//a.gameObject.SetActive(false);
			}
			rightHand.GetComponent<Animator>().SetTrigger("HideEverything");
			leftHand.GetComponent<Animator>().SetTrigger("HideEverything");
		}
		void OnDisable()
		{
			if (GameManager.IsApplicationQuitting)
				return;
			
			foreach (var a in anims)
			{
				a.gameObject.SetActive(false);
			}
			GameManager.Instance.playerStart -= OnPlayerStart;
		}
		public void ShowFinger(int side)
		{
			if(GameData.PlayerData.showControlsTutorial)
			{
				tutorialOverlay.SetActive(true);
				if (side < 1)
				{
					leftHand.GetComponent<Animator>().SetBool("Tutorial", true);
					leftHand.GetComponent<Animator>().SetTrigger("AnimateIn");
					return;
				}
				rightHand.GetComponent<Animator>().SetBool("Tutorial", true);
				rightHand.GetComponent<Animator>().SetTrigger("AnimateIn");
			}
		}
		public void HideFingers()
		{
			rightHand.GetComponent<Animator>().SetBool("Tutorial", false);
			leftHand.GetComponent<Animator>().SetBool("Tutorial", false);
			rightHand.GetComponent<Animator>().SetTrigger("GameStart");
			leftHand.GetComponent<Animator>().SetTrigger("GameStart");
		}
		public void DisplayPressed(int side)
		{
			if(side < 1)
			{
				leftHand.GetComponent<Animator>().SetTrigger("Pressed");
				return;
			}
			rightHand.GetComponent<Animator>().SetTrigger("Pressed");
		}

		public void UpdateFeedbackPoints(int p)
		{
			//feedbackPoints.text = "+" + p;
			//feedbackPoints.GetComponent<Animator>().Play("ScorePoints");
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (!CanvasController.HasOpenPopups){
					Pause();
				}            
			}
		}

		public void Pause()
		{
			if (GameManager.Instance.currentLevel.player.state != PlayerController.State.Normal)
			{
				return;
			}
		
			GameManager.Instance.TogglePause(true);
			CanvasController.Open(CanvasController.Popups.Pause);
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus == false)
			{
				if (!CanvasController.HasOpenPopups )
				{
					//Pause();
				}
			}		
		}
		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				// Pause();
			}
		}
	}
}

