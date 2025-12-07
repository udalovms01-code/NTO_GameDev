using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;
using System.IO;

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
[System.Serializable]
public class FishSpawnProperties
{
    public string id;
    public int position;
    public FishDirection direction;
}

public class VisualConfig
{
    public float animationSpeed = 1f;
}

public class Main : MonoBehaviour
{
    [FormerlySerializedAs("hand")] public FishZone field;

    public bool Testing = false;
    public bool Strategies = false;
    public float tableHeith = 0f;
    public Interactor interactor;

    public UnityAction<InteractiveObject> OnReleaseDrag;
    public UnityAction SceneChange; 
    public UnityAction OnGameEnd;
    
    
    

    public Animator  animator;
    private GameStateService _gameStateService;
    public FishStrategiesManager fishStrategiesManager;

    public CMSEntity levelEntity;
    public string setEntity;
    //public List<string> setsEntities;


    //public int fishCount = 0;
    //public List<InteractiveObject> fishes = new List<InteractiveObject>();


    public List<int> seed;
    public int seedPos = 0;
    List<string> levelSeq = new List<string>()
    {
        E.Id<Level1>(),
        //E.Id<Level2>()
    };

    private List<InteractiveObject> smartGeneratedObjects;

    void Awake()
    {
        if (!Testing)
        {
            if (_gameStateService.CurrentDay == 1)
            {
                G.run = null;
                G.visualConfig = null;
            }
        }
        else
        {
            G.run = null;
            G.visualConfig = null;
        }
        interactor = new Interactor();
        interactor.Init();


        if (G.run == null)
        {
            G.run = new RunState();

            G.run.maxHealth = 2;
            G.run.health = G.run.maxHealth;
            G.run.pointsSum = 0;
        }
        if (G.visualConfig == null)
        {
            G.visualConfig = new VisualConfig();

            G.visualConfig.animationSpeed = 1f;
        }

        fishStrategiesManager = new FishStrategiesManager();

        G.main = this;
        
        //ЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКАЗАГЛУШКА
        //setsEntities = new List<string>() {E.Id<EasySet>(), E.Id<EasySet>(), E.Id<EasySet>()};
    }
    
    [Inject]
    public void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    void Start()
    {
        if (Testing)
            StartCoroutine(TurnCoroutine());
    }

    public IEnumerator TurnCoroutine()
    {
        CMS.Init();
        
        G.OnGameReady?.Invoke();
        
        //fishCount = 0;
        
        
        CMSEntity levelToLoad;
    
        if (G.run.level < levelSeq.Count)
        {
            levelToLoad = CMS.Get<CMSEntity>(levelSeq[G.run.level]);
        }
        else
        {
            levelToLoad = CMS.Get<Level1>();
        }

        if (Strategies)
        {
            yield return SmartGenerateObjects();
            Debug.Log(smartGeneratedObjects);
        }
        else
        {
            yield return LoadLevel(levelToLoad);
        }
        
        yield break;
    }

    public void StartGame()
    {
        if (G.run.hasBaggage)
        {
            StartCoroutine(TurnCoroutine());
        }
    }

    void Update()
    {
        if (!Testing)
            if (_gameStateService != null && _gameStateService.CurrentState != GameState.SlicedFish) return;
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
            AddFish<GuppyFish>();

            G.feel.UIPunchSoft();
        }
        
        _gameStateService.SetHunger(_gameStateService.Hunger - (Time.deltaTime / 100));
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCoroutine());
    }

    IEnumerator EndTurnCoroutine()
    {
        G.hud.DisableHud();

        yield return AllFishesAcivation();

        yield return AllFishesCut();


        //G.run.set++;
        if (G.run.pointsSum < levelEntity.Get<TagLevelContent>().totalPoints)//(G.run.set < setsEntities.Count)
        {
            yield return DrawFish();
        }

        if (G.run.pointsSum >= levelEntity.Get<TagLevelContent>().totalPoints)//(G.run.set >= setsEntities.Count)
        {
            OnGameEnd?.Invoke();
        }
        else
        {
            G.hud.EnableHud();
        }
    }

    IEnumerator AllFishesAcivation()
    {
        int toActivate = field.objects.Count;//fishCount;
        field.FreezeAligning();

        for (int i = 0; i < toActivate; i++)
        {
            if (field.objects.Count <= i) continue;
            var fish = field.objects[i];
            if (fish == null) continue;
            G.hud.ArrowSelect(fish.transform.position + Vector3.forward, duration: .1f);

            yield return new WaitForSeconds(.4f * G.visualConfig.animationSpeed);
            yield return fish.Activate();
            

            /*
            if (fish.state.virused)
                yield return field.TryToInfect(fish);
            else
                yield return field.TryToEat(fish);
        */
            yield return field.TryToEat(fish);
        }
        G.hud.ArrowDisappear();
    }

    IEnumerator AllFishesCut()
    {
        int cutPerTurn = levelEntity.Get<TagDifficulty>().cutPerTurn;//fishCount;
        int toDel = cutPerTurn == -1 ? field.objects.Count : cutPerTurn;

        field.Align();
        
        yield return new WaitForSeconds(1f * G.visualConfig.animationSpeed);
        
        animator.SetTrigger("CameraOut");
        
        yield return new WaitForSeconds(1 * G.visualConfig.animationSpeed);
        
        animator.SetTrigger("CameraKnifeIn");
        
        yield return new WaitForSeconds(0.3f * G.visualConfig.animationSpeed);
        
        for (int i = 0; i < toDel; i++)
        {
            G.main.field.AlignSetForCutting();
            animator.SetTrigger("Cut");
            yield return new WaitForSeconds(0.55f * G.visualConfig.animationSpeed);
            G.feel.UIPunchSoft();
            yield return field.objects[field.objects.Count - 1].CutCoroutine();/*fishCount - 1].CutCoroutine();*/
            field.objects.RemoveAt(field.objects.Count - 1);/*fishCount - 1);*/
            yield return new WaitForSeconds(.35f * G.visualConfig.animationSpeed);
        }

        field.Align();
        animator.SetTrigger("CameraKnifeOut");

        yield return new WaitForSeconds(0.3f * G.visualConfig.animationSpeed);
        G.main.field.UnreezeAligning();
        
        animator.SetTrigger("CameraIn");
        yield return new WaitForSeconds(1 * G.visualConfig.animationSpeed);
    }

    IEnumerator SmartGenerateObjects()
    {
        G.visualConfig.animationSpeed = 0f;
        
        int startPoints;
        int startSum;
        int runsToSaveLast = 0;
        List<InteractiveObject> newObjects;
        for (int i = 0; i < 1000; i++)
        {
            startSum = G.run.pointsSum;
            G.run.set = 0; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            CMSEntity levelToLoad;
            if (G.run.level < levelSeq.Count)
            {
                levelToLoad = CMS.Get<CMSEntity>(levelSeq[G.run.level]);
            }
            else
            {
                levelToLoad = CMS.Get<Level1>();
            }
            levelEntity = levelToLoad;

            if (levelEntity.Is<TagLevelContent>(out var lc))
                setEntity = lc.startSet;
            else
                setEntity = E.Id<EasySet>();
            yield return DrawFish();
            newObjects = field.objects.ToList();
            yield return AllFishesAcivation();
            yield return AllFishesCut();

            startPoints = G.run.pointsSum - startSum;
            G.run.pointsSum = startSum;


            if (startPoints <= 0)
                continue;


            if (i == 999)
            {
                SceneChange?.Invoke();
                SceneManager.LoadScene(0);
            }

            for (int j = 0; j < 20; j++)
            {
                yield return DrawFish(newObjects.OrderBy(x => Random.value).ToList());
                
                yield return AllFishesAcivation();
                yield return AllFishesCut();

                startPoints = G.run.pointsSum - startSum;
                G.run.pointsSum = startSum;

                if (startPoints <= 0)
                {
                    smartGeneratedObjects = newObjects;
                    
                    if (runsToSaveLast < 2)
                    {
                        fishStrategiesManager.allRuns.runs[runsToSaveLast] = new FishRun();

                        for (int fishIndex = 0; fishIndex < 6; fishIndex++)
                        {
                            fishStrategiesManager.allRuns.runs[runsToSaveLast].fishes[fishIndex] = new FishSpawnProperties
                            {
                                id = newObjects[fishIndex].state.model.id,
                                position = fishIndex,
                                direction = newObjects[fishIndex].state.direction
                            };
                        }

                        runsToSaveLast++;
                    }
                    else
                    {
                        fishStrategiesManager.SaveFishData(fishStrategiesManager.allRuns);
                        G.visualConfig.animationSpeed = 1f;
                        yield break;
                    }
                }
            }

        }

        smartGeneratedObjects = null;
        G.visualConfig.animationSpeed = 1f;
    }

    
    public IEnumerator LoadLevel(CMSEntity entity)
    {
        G.run.set = 0;
        //fishCount = 0;
        
        levelEntity = entity;
        
        if (levelEntity.Is<TagLevelContent>(out var lc))
            setEntity = lc.startSet;
        else
            setEntity = E.Id<EasySet>();

        seed = FishStrategiesManager.GetShuffledList(fishStrategiesManager.strategiesCount);
        seedPos = 0;

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
        if (!Strategies)
        {
            G.audio.Play<SFX_DiceDraw>();
            int dice_count = 6; //fishCount;

            if (seedPos >= seed.Count)
            {
                yield return EndGame();
                yield break;
            }

            FishRun props = fishStrategiesManager.LoadFishData().runs[seed[seedPos]];

            for (int i = props.fishes.Length - 1; i >= 0; i--)
            {
                AddFish(props.fishes[i].id, direction: props.fishes[i].direction);
            }

            seedPos++;
            //ChooseVirusedFish();
            yield break;
        }
        else
        {
            if (CMS.Get<CMSEntity>(setEntity).Is<TagSetDefinition>(out var sd))
            {
                int dice_count = sd.fishOnTheBoardCount - field.objects.Count; //fishCount;
                for (int i = 0; i < dice_count; i++)
                {
                    float seed = Random.Range(0f, 1f);
                    float cursum = 0;
                    foreach (var data in sd.fish_probabilities)
                    {
                        cursum += data.Value;
                        if (seed <= cursum)
                        {
                            AddFish(data.Key);
                            break;
                        }
                    }
                }

                //ChooseVirusedFish();
                yield break;
            }
            else
            {
                int dice_count = sd.fishOnTheBoardCount - field.objects.Count;
                for (var i = 0; i < dice_count; i++)
                {
                    AddFish<GuppyFish>();
                    yield return new WaitForSeconds(0.2f * G.visualConfig.animationSpeed);
                }
            }
        }
    }

    IEnumerator DrawFish(List<InteractiveObject> newObjects)
    {
        foreach (var obj in newObjects)
        {
            AddFish(obj.state.model.id);
        }
        yield break;
    }

    public void ChooseVirusedFish()
    {
        bool noVirusFlag = true;
        for (int i = 0; i < field.objects.Count; i++)/*fishCount; i++)*/
        {
            if (!field.objects[i].state.model.Is<TagCannotBeVirused>())
            {
                noVirusFlag = false;
                break;
            }
        }

        if (noVirusFlag) return;
        int virusedInd = Random.Range(0, field.objects.Count - 1);//fishCount - 1);
        while (field.objects[virusedInd].state.model.Is<TagCannotBeVirused>())
            virusedInd = Random.Range(0, field.objects.Count - 1);/*fishCount - 1);*/
        
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

    public void AddFish(string t, bool strategies = false, FishDirection direction = FishDirection.Right)
    {
        var basicDice = CMS.Get<CMSEntity>(t);
        var state = new FishState();
        state.model = basicDice;
        var instance = Instantiate(basicDice.Get<TagPrefab>().prefab, G.main.gameObject.transform);
        if (strategies)
            instance.InitState(state, true, direction);
        else
            instance.InitState(state);
        field.Claim(instance);
        //fishCount++;
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
    
    public void AddHunger(float value)
    {
        if (!Testing)
            _gameStateService.SetHunger(_gameStateService.Hunger + value);
    }
    
    public IEnumerator EndGame()
    {
        StopAllCoroutines();
        OnGameEnd?.Invoke();
        G.hud.DisableHud();
        yield break;
    }

    /*public void DeleteFish(InteractiveObject interactiveObject)
    {
        int index = fishes.IndexOf(interactiveObject);
        fishes[index].spriteAnimator.Punch();
        fishes.Remove(interactiveObject);
        fishCount--;
    }*/
}

[System.Serializable]
public class FishRun
{
    public FishSpawnProperties[] fishes = new FishSpawnProperties[6];
    public FishRun()
    {
        fishes = new FishSpawnProperties[6];
        // Инициализируем каждый элемент
        for (int i = 0; i < 6; i++)
        {
            fishes[i] = new FishSpawnProperties();
        }
    }
}

[System.Serializable]
public class FishRunsData
{
    public FishRun[] runs = new FishRun[2];
}

public class FishStrategiesManager
{
    private string filePath = "";
    public FishRunsData allRuns = new FishRunsData();
    public int strategiesCount = 2;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "fishdata.json");
    }

    public void SaveFishData(FishRunsData data)
    {
        string safePath = Path.Combine(Application.persistentDataPath, "fishdata.json");
        filePath = safePath;
        
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(safePath, json);
        Debug.Log($"Данные сохранены в: {safePath}");
    }

    public FishRunsData LoadFishData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("strategies");
        string json = textAsset.text;
        FishRunsData data = JsonUtility.FromJson<FishRunsData>(json);
        //Debug.Log("Данные загружены успешно!");
        return data;
    }
    
    public static List<int> GetShuffledList(int n)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < n; i++)
        {
            list.Add(i);
        }
        
        // Fisher-Yates shuffle (с конца к началу)
        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        
        return list;
    }
}