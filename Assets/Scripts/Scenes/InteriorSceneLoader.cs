using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteriorSceneLoader : MonoBehaviour
{
    [Header("Cenas de interior")]
    [Tooltip("Nomes das cenas carregadas junto com a cidade ao iniciar (iguais aos arquivos, sem .unity). Precisam estar na Scene List do Build Profiles.")]
    [SerializeField] private string[] interiorSceneNames;

    public event Action InteriorsLoaded;
    public bool AreInteriorsLoaded { get; private set; }

    private void Start()
    {
        StartCoroutine(LoadInteriors());
    }

    private IEnumerator LoadInteriors()
    {
        foreach (string sceneName in interiorSceneNames)
        {
            if (!CanLoad(sceneName)) continue;

            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        AreInteriorsLoaded = true;

        InteriorsLoaded?.Invoke();
    }

    private bool CanLoad(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.isLoaded) return false;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"A cena '{sceneName}' não está na Scene List do Build Profiles.");
            return false;
        }

        return true;
    }
}
