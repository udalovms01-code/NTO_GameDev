using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Level0 : CMSEntity
{
    public Level0()
    {
        Define<TagLevelContent>().startSet = E.Id<EmptySet>();
        Define<TagLevelContent>().totalPoints = 3;
        Define<TagLevelContent>().levelTime = 1000f;
        Define<TagLevelScript>().toExecute = Script;
        Define<TagTutorialLevel>().set = E.Id<EmptySet>();
        Define<TagDifficulty>().virusPerSet = 1;
    }


    IEnumerator Script()
    {
        CMSEntity entity = CMS.Get<CMSEntity>(G.main.levelEntity.Get<TagTutorialLevel>().set);
        G.hud.DisableHud();
        
        G.main.PauseHunger();
        
        G.ui.click_to_continue.SetActive(true);
        yield return G.main.Say("Время обучения...");
        yield return G.main.SmartWait(5f);
        G.ui.click_to_continue.SetActive(false);


        yield return G.main.Say("Нужно приготовить пищу");
        G.ui.click_to_continue.SetActive(true);
        yield return G.main.SmartWait(5f);
        G.ui.click_to_continue.SetActive(false);
        yield return G.main.Say("Торопись! Не дай голоду упасть до 0");
        G.ui.click_to_continue.SetActive(true);
        yield return G.main.SmartWait(5f);
        G.ui.click_to_continue.SetActive(false);
        yield return G.main.Unsay();


        yield return G.main.DrawFish(entity.Get<TagTutorial1Set>().firstBoard);

        yield return G.main.Say("Это твой запас рыб");
        yield return G.main.SmartWait(5f);
        yield return G.main.Say("Чем больше качество рыб, тем больше голода они восстановят");
        yield return G.main.SmartWait(5f);
        yield return G.main.Say("Похоже ситуация так себе... Надо это исправить");
        yield return G.main.SmartWait(5f);
        yield return G.main.Unsay();
        
        G.hud.EnableHud();
        KnifeInteractive.interactable = false;
        
        yield return new WaitUntil(() => G.main.TutorFlag == true);
        G.main.TutorFlag = false;
        G.hud.DisableHud();
        
        
        yield return G.main.Say("Отлично!");
        yield return G.main.SmartWait(5f);
        yield return G.main.Say("Когда ты будешь готов, ты можешь использовать тесак и накормить чудище");
        yield return G.main.SmartWait(5f);
        
        KnifeInteractive.interactable = false;
        
        G.hud.EnableHud();
        
        yield return G.main.Unsay();
        G.main.UnpauseHunger();
        yield break;
    }
}

public class TagTutorialLevel : EntityComponentDefinition
{
    public string set;
}