using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    [System.Serializable]
    public class ModelsStage
    {
        public GameObject[] Models;
    }
    
    public class ShipVisualControl : MonoBehaviour
    {
        [SerializeField] private ModelsStage[] _activeModels;
        [SerializeField] private ModelsStage[] _inactiveModels;
        
        [Inject]
        private void Construct(GameStateService gameStateService)
        {
            gameStateService.OnDayChanged += OnDayChanged;
        }
        
        private void OnDayChanged(int day)
        {
            for (int i = 0; i < _activeModels.Length; i++)
            {
                Debug.Log(day);
                for (int j = 0; j < _activeModels[i].Models.Length; j++)
                {
                    _activeModels[i].Models[j].SetActive(day >= i+1);
                }

                for (int j = 0; j < _inactiveModels[i].Models.Length; j++)
                {
                    if(day >= i + 1) _inactiveModels[i].Models[j].SetActive(false);  
                }
            }
        }
    }
}