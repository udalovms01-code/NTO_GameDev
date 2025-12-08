using System.Collections.Generic;
using UnityEngine;

namespace UI.Endings
{
    [CreateAssetMenu(fileName = "EndingLibrary", menuName = "Endings/Ending Library")]
    public class EndingLibrary : ScriptableObject
    {
        [SerializeField] private List<EndingDefinition> endings = new();

        public IReadOnlyList<EndingDefinition> Endings => endings;
    }
}
