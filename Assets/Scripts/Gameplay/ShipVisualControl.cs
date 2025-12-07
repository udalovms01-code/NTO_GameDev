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
        [SerializeField] private ModelsStage[] _models;
        
        [Inject]
        private void Construct(GameStateService gameStateService)
        {
            gameStateService.OnDayChanged += OnDayChanged;
        }
        
        private void OnDayChanged(int day)
        {
            for (int i = 0; i < _models.Length; i++)
            {
                for (int j = 0; j < _models[i].Models.Length; j++)
                {
                    _models[i].Models[j].SetActive(i == day);
                }
            }
        }
    }
}