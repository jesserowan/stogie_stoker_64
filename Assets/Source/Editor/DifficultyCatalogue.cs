using UnityEditor;
using UnityEngine;

[FilePath("SomeSubFolder/StateFile.foo", FilePathAttribute.Location.PreferencesFolder)]
public class DifficultyCatalogue: ScriptableSingleton<DifficultyCatalogue>
{
    [SerializeField] public DifficultyValue Hard;
    [SerializeField] public DifficultyValue Easy;
    [SerializeField] public DifficultyValue Mid;
}
