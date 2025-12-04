using System;

namespace Gameplay
{
    public class GameStateService
    {
        public bool IsTutorialCompleted { get; private set; }
        public bool IsDialogEnded { get; private set; }
        public bool IsFishesSliced { get; private set; }
        public int CurrentDay { get; private set; } = 1;
        
        public event Action<bool> OnTutorialCompleted;
        public event Action<bool> OnDialogEnded;
        public event Action<bool> OnFishesSliced;
        public event Action<int> OnDayChanged;
        
        public void SetDay(int day)
        {
            if (CurrentDay == day) return;
            
            SetFishesSliced(false);
            SetDialogEnded(false);
            
            CurrentDay = day;
            OnDayChanged?.Invoke(day);
        }

        public void SetTutorialCompleted(bool value)
        {
            if (IsTutorialCompleted == value) return;

            IsTutorialCompleted = value;
            OnTutorialCompleted?.Invoke(value);
        }
        
        public void SetDialogEnded(bool value)
        {
            if (IsDialogEnded == value) return;

            IsDialogEnded = value;  
            OnDialogEnded?.Invoke(value);
        }
        
        public void SetFishesSliced(bool value)
        {
            if (IsFishesSliced == value) return;    

            IsFishesSliced = value;
            OnFishesSliced?.Invoke(value);
        }
    }
}