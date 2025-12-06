using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class HungerUI : MonoBehaviour
    {
        public Slider Hunger;

        
        
        
        void Awake()
        {
            G.HungerUI = this;
        }

        void Start()
        {
            StartCoroutine(TrackHealth());
        }
        
        
        IEnumerator TrackHealth()
        {
            Hunger.value = G.run.pointsSum / 2;
            yield return UpdateHP();

            while (true)
            {
                if (Hunger.value > G.run.pointsSum)
                {
                    Hunger.value--;
                    yield return UpdateHP();
                }

                if (Hunger.value < G.run.pointsSum)
                {
                    Hunger.value++;
                    yield return UpdateHP();
                }

                Hunger.maxValue = G.run.pointsSum;
                yield return new WaitForEndOfFrame();
            }
        }
        
        IEnumerator UpdateHP()
        {
            //yield return G.ui.ScaleCountIn(HealthValue.transform);
            //HealthValue.text = Health.value + "/" + G.run.maxHealth;
            //yield return G.ui.ScaleCountOut(HealthValue.transform);
            yield break;
        }
    }
}