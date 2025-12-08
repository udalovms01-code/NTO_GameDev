using System;
using Gameplay;
using UnityEngine;
using Zenject;

namespace UI
{
    public class MonsterStage : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Inject]
        private void Construct(GameStateService gameStateService)
        {
            gameStateService.OnDayChanged += (state) => _spriteRenderer.sprite = _sprites[state-1];
        }
    }
}