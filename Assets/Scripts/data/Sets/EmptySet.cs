using System.Collections;
using System.Collections.Generic;

public class EmptySet : CMSEntity
{
    public EmptySet()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;
        Define<TagEmptySet>();
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<TutorBreamFish>());
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<ClownFish>());
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<ClownFish>());
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<ClownFish>());
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<TutorBreamFish>());
        Define<TagTutorial1Set>().firstBoard.Add(E.Id<ClownFish>());
    }
}

public class TagEmptySet : EntityComponentDefinition
{
    
}

public class TagTutorial1Set : EntityComponentDefinition
{
    public List<string> firstBoard =  new List<string>();
}