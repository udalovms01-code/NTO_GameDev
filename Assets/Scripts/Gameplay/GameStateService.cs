using System;
using UnityEngine;

namespace Gameplay
{
    public enum GameState
    {
        WaitingForSlicedFish = 0,
        SlicedFish,
        WaitingForDialog,
        Dialog,
        WaitingForSleep,
        Sleep
    }
    
    public class GameStateService
    {
        public GameState CurrentState { get; private set; }
        public bool IsTutorialCompleted { get; private set; }
        public int CurrentDay { get; private set; } = 1;
        public bool IsFishesSlicedStarted { get; private set; }
        public float Hunger { get; private set; }
        
        public event Action<bool> OnTutorialCompleted;
        public event Action<int> OnDayChanged;
        public event Action<float> OnHungerChanged;
        public event Action<GameState> OnStateChanged;
        
        public void SetHunger(float value)
        {
            if (Mathf.Approximately(Hunger, value)) return;
            
            Hunger = value;
            OnHungerChanged?.Invoke(value);
        }
        
        public void SetDay(int day)
        {
            if (CurrentDay == day) return;
            
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
    }
}