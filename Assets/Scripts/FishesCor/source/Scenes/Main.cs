using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class RunState
{
    public int level = 0;
    public int set = 0;
    public int drawSize = 6;
    public int pointsSum = 0;
    
    public int health = 2;
    public int maxHealth = 2;

    public bool hasBaggage = true;
}

public class Main : MonoBehaviour
{
    [FormerlySerializedAs("hand")] public FishZone field;

    public bool Testing = false;
    public float tableHeith = 0f;
    public Interactor interactor;

    public UnityAction<InteractiveObject> OnReleaseDrag;
    public UnityAction SceneChange; 
    public UnityAction OnGameEnd;
    
    
    

    public Animator  animator;
    private GameStateService _gameStateService;

    public CMSEntity levelEntity;
    public List<string> setsEntities;


    public int fishCount = 0;
    
    List<string> levelSeq = new List<string>()
    {
        E.Id<Level1>(),
        E.Id<Level2>()
    };

    void Awake()
    {
        interactor = new Interactor();
        interactor.Init();
        
        if (G.run == null)
        {
            G.run = new RunState();

            G.run.maxHealth = 2;
            G.run.health = G.run.maxHealth;
        }

        G.main = this;
        
        //ЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКА
        setsEntities = new List<string>() {E.Id<EasySet>(), E.Id<EasySet>(), E.Id<EasySet>()};
    }
    
    [Inject]
    public void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    void Start()
    {
        if (Testing)
            StartGame();
    }

    public void StartGame()
    {
        if (G.run.hasBaggage)
            StartCoroutine(Turn());
    }

    public IEnumerator Turn()
    {
        CMS.Init();
        
        G.OnGameReady?.Invoke();
        
        fishCount = 0;
        
        if (G.run.level < levelSeq.Count)
            yield return LoadLevel(CMS.Get<CMSEntity>(levelSeq[G.run.level]));
        else
            SceneManager.LoadScene("ldgame/end_screen");
        
        yield break;
    }

    void Update()
    {
        if (_gameStateService is { IsFishesSlicedStarted: false }) return;
        G.ui.debug_text.text = "";
        G.ui.debug_text.text += "R-reload\n";
        G.ui.debug_text.text += "D-add dice\n";
        G.ui.debug_text.text += "I-reload with intro\n";

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneChange?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            /*InteractiveObject isv = "prefab/fish_view".Load<InteractiveObject>();
            Instantiate(isv);*/
            AddFish<BasicFish>();
            /*if (Random.Range(0f, 1f) < 0.5f)
                AddDice<BasicDice>();
            else
                AddDice<FudgeDice>();*/

            G.feel.UIPunchSoft();
        }
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCoroutine());
    }

    IEnumerator EndTurnCoroutine()
    {
        G.hud.DisableHud();

        int toDel = fishCount;
        field.FreezeAligning();

        for (int i = 0; i < toDel; i++)
        {
            var fish = field.objects[i];
            if (fish == null) continue;
            G.hud.ArrowSelect(fish.transform.position + Vector3.forward, duration: .1f);

            yield return new WaitForSeconds(.4f);
            fish.Activate.Invoke();
            var endTurn = G.main.interactor.FindAll<IOnEndTurn>();
            foreach (var et in endTurn)
                yield return et.OnEndTurn(fish.state);

            if (fish.state.virused)
                yield return field.TryToInfect(fish);
            else
                yield return field.TryToEat(fish);
        }
        G.hud.ArrowDisappear();
        
        toDel = fishCount;

        field.Align();
        
        yield return new WaitForSeconds(1);
        
        animator.SetTrigger("CameraOut");
        
        yield return new WaitForSeconds(1);
        
        animator.SetTrigger("CameraKnifeIn");
        
        yield return new WaitForSeconds(0.3f);
        
        for (int i = 0; i < toDel; i++)
        {
            G.main.field.AlignSetForCutting();
            animator.SetTrigger("Cut");
            yield return new WaitForSeconds(0.55f);
            G.feel.UIPunchSoft();
            field.objects[fishCount - 1].Cut();
            field.objects.RemoveAt(fishCount - 1);
            yield return new WaitForSeconds(.35f);
        }
        animator.SetTrigger("CameraKnifeOut");
        
        yield return new WaitForSeconds(0.3f);
        
        G.main.field.UnreezeAligning();

        G.run.set++;
        if (G.run.set < setsEntities.Count)
        {
            yield return DrawFish();
        }


        animator.SetTrigger("CameraIn");
        yield return new WaitForSeconds(1);


        if (G.run.set >= setsEntities.Count)
        {
            OnGameEnd?.Invoke();
        }
        else
        {
            G.hud.EnableHud();
        }
    }
    public IEnumerator LoadLevel(CMSEntity entity)
    {
        G.run.set = 0;
        levelEntity = entity;

        yield return DrawFish();

        if (levelEntity.Is<TagLevelScript>(out var exs))
        {
            yield return exs.toExecute();
        }

        yield return new WaitForSeconds(.5f);
        
        G.hud.EnableHud();
    }


    private IEnumerator DrawFish()
    {
        G.audio.Play<SFX_DiceDraw>();
        if (CMS.Get<CMSEntity>(setsEntities[G.run.set]).Is<TagSetDefinition>(out var sd))
        {
            for (int i = 0; i < sd.fishCount; i++)
            {
                float seed = Random.Range(0f, 1f);
                float cursum = 0;
                foreach (var data in sd.datas)
                {
                    cursum += data.percentage;
                    if (seed <= cursum)
                    {
                        AddFish(data.fish);
                        break;
                    }
                }
            }
            
            ChooseVirusedFish();
            yield break;
        }
        else
        {
            for (var i = 0; i < G.run.drawSize; i++)
            {
                AddFish<BasicFish>();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void ChooseVirusedFish()
    {
        bool noVirusFlag = true;
        for (int i = 0; i < fishCount; i++)
        {
            if (!field.objects[i].state.model.Is<TagCannotBeVirused>())
            {
                noVirusFlag = false;
                break;
            }
        }

        if (noVirusFlag) return;
        int virusedInd = Random.Range(0, fishCount - 1);
        while (field.objects[virusedInd].state.model.Is<TagCannotBeVirused>())
            virusedInd = Random.Range(0, fishCount - 1);
        
        field.objects[virusedInd].MakeVirused();
    }

    public void TryPlayDice(InteractiveObject dice)
    {
        StartCoroutine(PlayDice(dice));
    }

    IEnumerator PlayDice(InteractiveObject dice)
    {
        field.Claim(dice);

        yield return new WaitForSeconds(0.25f);

        var roll = 1 + Random.Range(0, 6);
        dice.SetValue(roll);
        G.feel.UIPunchSoft();

        yield return new WaitForSeconds(0.25f);

        var onPlayDice = interactor.FindAll<IOnPlace>();
        foreach (var onPlay in onPlayDice)
            yield return onPlay.OnPlace(dice.state);
    }

    public void AddFish<T>() where T : CMSEntity
    {
        AddFish(E.Id<T>());
    }

    public void AddFish(string t)
    {
        var basicDice = CMS.Get<CMSEntity>(t);
        var state = new FishState();
        state.model = basicDice;
        var instance = Instantiate(basicDice.Get<TagPrefab>().prefab, G.main.gameObject.transform);
        instance.SetState(state);
        field.Claim(instance);
        fishCount++;
    }

    public void StartDrag(DraggableSmoothDamp draggableSmoothDamp)
    {
        G.drag_dice = draggableSmoothDamp.GetComponent<InteractiveObject>();
    }

    public void StopDrag()
    {
        OnReleaseDrag?.Invoke(G.drag_dice);
        G.drag_dice = null;
    }
    
    public IEnumerator EndGame()
    {
        yield break;
    }
}