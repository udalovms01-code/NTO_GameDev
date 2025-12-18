using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
    public enum GameState
    {
        WaitingForSlicedFish,
        SlicedFish,
        WaitingForDialog,
        Dialog,
        WaitingForSleep,
        Sleep
    }
    
    public class GameStateService : ISaveDataSource, IInitializable
    {
        public GameState CurrentState { get; private set; } = GameState.Sleep;
        public bool IsTutorialCompleted { get; private set; }
        public int CurrentDay { get; private set; }
        public bool IsFishesSlicedStarted { get; private set; }
        public float Hunger { get; private set; } = 1f;
        
        public static event Action<bool> OnTutorialCompleted;
        public event Action<int> OnDayChanged;
        public event Action<float> OnHungerChanged;
        public event Action<float> OnHungerChangedByFishvalue;
        public event Action<GameState> OnStateChanged;
        
        public void SetHunger(float value)
        {
            if (Mathf.Approximately(Hunger, value)) return;
            
            Hunger = value;
            Hunger = Mathf.Max(Mathf.Min(Hunger, 1f), 0f);
            OnHungerChanged?.Invoke(value);
        }

        public void AddHungerByFishvalue(float val)
        {
            OnHungerChangedByFishvalue?.Invoke(val);
        }
        
        public void SetDay(int day)
        {
            if (CurrentDay == day) return;
            if (CurrentDay == 8)
            {
                FadeController.Instance.FadeIn(() =>
                {
                    FadeController.Instance.FadeOut();
                    SceneManager.LoadScene(0);
                });
                return;
            }
            
            SetState(GameState.WaitingForSlicedFish);
            CurrentDay = day;
            OnDayChanged?.Invoke(day);
        }
        
        public void SetState(GameState state)
        {
            if (CurrentState == state) return;
            CurrentState = state;
            OnStateChanged?.Invoke(state);
        }

        public void SetTutorialCompleted(bool value)
        {
            if (IsTutorialCompleted == value) return;

            IsTutorialCompleted = value;
            OnTutorialCompleted?.Invoke(value);
        }

        public void Capture(SaveDataContainer container)
        {
            container.gameplay.CurrentState = CurrentState;
            container.gameplay.CurrentDay = CurrentDay;
            container.gameplay.Hunger = Hunger;
        }

        public void Restore(SaveDataContainer container)
        {
            SetState(container.gameplay.CurrentState);
            SetDay(container.gameplay.CurrentDay);
            SetHunger(container.gameplay.Hunger);
        }

        public void Initialize()
        {
            SetDay(1);
        }
    }
}