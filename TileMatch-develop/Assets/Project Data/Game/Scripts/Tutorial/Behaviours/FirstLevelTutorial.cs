using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Watermelon
{
    public class FirstLevelTutorial : BaseTutorial, ITutorial
    {
        private const int STEP_1_PICK_APPLES = 0;
        private const int STEP_2_PICK_CHEESE = 1;
        private const int STEP_3_DEPTH = 2;
        private const int STEP_4_PRE_HINT_DELAY = 3;
        private const int STEP_5_HINT = 4;
        private const int STEP_6_SHUFFLE = 5;
        private const int STEP_7_PICK_ELEMENT = 6;
        private const int STEP_8_UNDO = 7;
        private const int STEP_9_FINISH = 8;

        private static FirstLevelTutorial tutorialController;

        [SerializeField] BackgroundData backgroundData;
        [SerializeField] Color tileDisableColor;

        [Header("Step I")]
        [SerializeField] LevelData firstLevelData;
        [SerializeField] PreloadedLevelData firstPreloadedLevelData;
        [SerializeField] string firstStepTitle = "Welcome";
        [SerializeField] string firstStepMessage = "Clear the game field";

        [Header("Step II")]
        [SerializeField] string secondStepTitle = "Great";
        [SerializeField] string secondStepMessage = "Let's merge 3 other tiles";

        [Header("Step III")]
        [SerializeField] LevelData thirdLevelData;
        [SerializeField] PreloadedLevelData thirdPreloadedLevelData;
        [SerializeField] string thirdStepTitle = "Unlock tile";
        [SerializeField] string thirdStepMessage = "On the lower layer";

        [Header("Step IV")]
        [SerializeField] string fourthStepTitle = "Great";
        [SerializeField] string fourthStepMessage = "";

        [Header("Step V")]
        [SerializeField] string fifthStepTitle = "Use a hint";
        [SerializeField] string fifthStepMessage = "If you get stuck";

        [Header("Step VI")]
        [SerializeField] string sixthStepTitle = "Shuffle";
        [SerializeField] string sixthStepMessage = "If it's impossible to make more moves";

        [Header("Step VII")]
        [SerializeField] string seventhStepTitle = "Great!";
        [SerializeField] string seventhStepMessage = "Let's finish the level!";

        [Header("Step IIX")]
        [SerializeField] string eighthStepTitle = "Undo";
        [SerializeField] string eighthStepMessage = "If you did the wrong move";

        [Header("Step IX")]
        [SerializeField] string ninthStepTitle = "Great!";
        [SerializeField] string ninthStepMessage = "Complete the level to continue";

        [Header("Finish")]
        [SerializeField] string finishTitle = "Good job!";

        private bool isActive;
        public override bool IsActive => isActive;

        private int progress;
        public override int Progress => progress;

        public override bool IsFinished => saveData.isFinished;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private List<TileBehavior> cheeseTiles;
        private List<TileBehavior> appleTiles;

        private TileBehavior pointerTile;
        private TileBehavior pyramidTile;
        private TileBehavior undoTile;
        private List<TileBehavior> undoClickableTiles;

        public override void Initialise()
        {
            tutorialController = this;

            saveData = SaveController.GetSaveObject<TutorialBaseSave>(string.Format(ITutorial.SAVE_IDENTIFIER, TutorialID.ToString()));

            gameUI = UIController.GetPage<UIGame>();
        }

        public override void StartTutorial()
        {
            if (isActive) return;

            isActive = true;
            progress = 0;

            EnableStep(0);

            DockBehavior.MatchCombined += OnMatchCombined;
            DockBehavior.ElementAdded += OnElementAddedToDock;
            PUController.OnPowerUpUsed += OnPUUsed;

            AdsManager.DisableBanner();
        }

        private void OnPUUsed(PUType powerUpType)
        {
            if(progress == STEP_5_HINT)
            {
                EnableStep(STEP_6_SHUFFLE);
            }
            else if (progress == STEP_6_SHUFFLE)
            {
                EnableStep(STEP_7_PICK_ELEMENT);
            }
            else if(progress == STEP_8_UNDO)
            {
                EnableStep(STEP_9_FINISH);
            }
        }

        private void EnableStep(int stepIndex)
        {
            if (stepIndex == STEP_1_PICK_APPLES)
            {
                gameUI.SetTutorialText(firstStepTitle, firstStepMessage);

                GameController.LoadCustomLevel(tutorialController.firstLevelData, tutorialController.firstPreloadedLevelData, tutorialController.backgroundData, true, () =>
                {
                    // Get cheese tiles
                    cheeseTiles = new List<TileBehavior>();
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(0, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(1, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(2, 0, 1)));

                    foreach (var cheese in cheeseTiles)
                    {
                        cheese.SetBlockState(true);
                        cheese.SetColor(tileDisableColor, true);
                    }

                    // Get apple tiles
                    appleTiles = new List<TileBehavior>();
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(0, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(1, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(2, 1, 1)));

                    foreach (var apple in appleTiles)
                    {
                        apple.SetBlockState(false);
                    }

                    ActivateTilePointer(appleTiles[0]);
                });
            }
            else if (stepIndex == STEP_2_PICK_CHEESE)
            {
                gameUI.SetTutorialText(secondStepTitle, secondStepMessage);

                foreach (var cheese in cheeseTiles)
                {
                    cheese.SetBlockState(false);
                    cheese.SetState(true, true);
                }

                ActivateTilePointer(cheeseTiles[0]);
            }
            else if (stepIndex == STEP_3_DEPTH)
            {
                gameUI.SetTutorialText(thirdStepTitle, thirdStepMessage);

                GameController.LoadCustomLevel(tutorialController.thirdLevelData, tutorialController.thirdPreloadedLevelData, tutorialController.backgroundData, false, () =>
                {
                    pyramidTile = LevelController.GetTile(new ElementPosition(1, 1, 0));

                    ActivateTilePointer(pyramidTile);

                    TileBehavior dockTile = LevelController.SpawnDockTile(0);

                    Vector3 tileSize = dockTile.transform.localScale;
                    dockTile.transform.localScale = Vector3.zero;
                    dockTile.transform.DOScale(tileSize, 0.5f).SetEasing(Ease.Type.BackOut);
                });
            }
            else if (stepIndex == STEP_4_PRE_HINT_DELAY)
            {
                gameUI.SetTutorialText(fourthStepTitle, fourthStepMessage);

                RaycastController.Disable();

                Tween.DelayedCall(0.6f, () =>
                {
                    EnableStep(STEP_5_HINT);
                });
            }
            else if (stepIndex == STEP_5_HINT)
            {
                gameUI.SetTutorialText(fifthStepTitle, fifthStepMessage);

                PUUIBehavior hintPanel = PUController.PowerUpsUIController.GetPanel(PUType.Hint);
                hintPanel.gameObject.SetActive(true);
                hintPanel.Settings.Save.Amount = 1;
                hintPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(hintPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_6_SHUFFLE)
            {
                TutorialCanvasController.ResetPointer();

                gameUI.SetTutorialText(sixthStepTitle, sixthStepMessage);

                PUController.PowerUpsUIController.HidePanel(PUType.Hint);

                PUUIBehavior shufflePanel = PUController.PowerUpsUIController.GetPanel(PUType.Shuffle);
                shufflePanel.gameObject.SetActive(true);
                shufflePanel.Settings.Save.Amount = 1;
                shufflePanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(shufflePanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                }, 2);
            }
            else if (stepIndex == STEP_7_PICK_ELEMENT)
            {
                gameUI.SetTutorialText(seventhStepTitle, seventhStepMessage);

                TutorialCanvasController.ResetPointer();

                undoClickableTiles = new List<TileBehavior>(LevelController.LevelRepresentation.Tiles);
                for(int i = 0; i < undoClickableTiles.Count; i++)
                {
                    if (undoTile == null && undoClickableTiles[i].IsClickable)
                    {
                        undoTile = undoClickableTiles[i];

                        undoClickableTiles.RemoveAt(i);

                        break;
                    }
                }

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(true);
                    tile.SetColor(tileDisableColor, true);
                }

                Tween.DelayedCall(0.3f, () =>
                {
                    ActivateTilePointer(undoTile);
                });

                PUController.PowerUpsUIController.HidePanel(PUType.Shuffle);
            }
            else if (stepIndex == STEP_8_UNDO)
            {
                gameUI.SetTutorialText(eighthStepTitle, eighthStepMessage);

                PUUIBehavior undoPanel = PUController.PowerUpsUIController.GetPanel(PUType.Undo);
                undoPanel.gameObject.SetActive(true);
                undoPanel.Settings.Save.Amount = 1;
                undoPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(undoPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_9_FINISH)
            {
                TutorialCanvasController.ResetPointer();

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(false);
                    tile.SetState(LevelController.LevelRepresentation.IsTileUnconcealed(tile), true);
                }

                gameUI.SetTutorialText(ninthStepTitle, ninthStepMessage);

                PUController.PowerUpsUIController.HidePanel(PUType.Undo);
            }

            progress = stepIndex;
        }

        private void ActivateTilePointer(TileBehavior tileBehavior)
        {
            if(tileBehavior != null)
            {
                TutorialCanvasController.ActivatePointer(tileBehavior.transform.position, TutorialCanvasController.POINTER_DEFAULT);

                pointerTile = tileBehavior;
            }
        }

        private void DisableTilePointer()
        {
            TutorialCanvasController.ResetPointer();

            pointerTile = null;
        }

        private void OnElementAddedToDock(ISlotable tile)
        {
            TileBehavior pickedTile = (TileBehavior)tile;
            if(pickedTile != null)
            {
                if (pickedTile == pointerTile)
                    DisableTilePointer();

                if(progress == STEP_1_PICK_APPLES)
                {
                    appleTiles.Remove(pickedTile);

                    if(appleTiles.Count > 0)
                    {
                        ActivateTilePointer(appleTiles[0]);
                    }
                    else
                    {
                        EnableStep(STEP_2_PICK_CHEESE);
                    }
                }
                else if(progress == STEP_2_PICK_CHEESE)
                {
                    cheeseTiles.Remove(pickedTile);

                    if (cheeseTiles.Count > 0)
                    {
                        ActivateTilePointer(cheeseTiles[0]);
                    }
                }
                else if(progress == STEP_3_DEPTH)
                {
                    if(pickedTile == pyramidTile)
                    {
                        EnableStep(STEP_4_PRE_HINT_DELAY);
                    }
                }
                else if (progress == STEP_7_PICK_ELEMENT)
                {
                    if (pickedTile == undoTile)
                    {
                        EnableStep(STEP_8_UNDO);
                    }
                }
            }
        }

        private void OnMatchCombined(List<ISlotable> tiles)
        {
            if (progress == STEP_2_PICK_CHEESE)
            {
                if (cheeseTiles.IsNullOrEmpty())
                {
                    EnableStep(STEP_3_DEPTH);
                }
            }
            else if(progress == STEP_9_FINISH)
            {
                if (LevelController.LevelRepresentation.Tiles.IsNullOrEmpty())
                {
                    gameUI.SetTutorialText(finishTitle, "");

                    Tween.DelayedCall(1.0f, () =>
                    {
                        CompleteTutorial();
                    });
                }
            }
        }

        public void CompleteTutorial()
        {
            FinishTutorial();

            gameUI.DisableTutorial();

            DockBehavior.MatchCombined -= OnMatchCombined;
            DockBehavior.ElementAdded -= OnElementAddedToDock;
            PUController.OnPowerUpUsed -= OnPUUsed;

            AdsManager.EnableBanner();

            LevelController.CompleteCustomLevel();

            GameController.LoadLevel(0, () =>
            {
                gameUI.PowerUpsUIController.ShowPanels();
            });
        }

        public override void FinishTutorial()
        {
            TutorialCanvasController.ResetPointer();

            PUBehavior[] powerUps = PUController.ActivePowerUps;
            foreach(var powerUp in powerUps)
            {
                powerUp.Settings.Save.Amount = powerUp.Settings.DefaultAmount;
            }

            saveData.isFinished = true;

            isActive = false;
        }

        public override void Unload()
        {

        }

        public void OnSkipButtonClicked()
        {
            if(isActive && !saveData.isFinished)
            {
                CompleteTutorial();
            }
        }
    }
}
