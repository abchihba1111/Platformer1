using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector director;
    public GameObject player;
    public GameObject mainCamera;
    public GameObject cutsceneCamera;

    void Start()
    {
        // Автоматически находим объекты
        if (director == null) director = GetComponent<PlayableDirector>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (mainCamera == null) mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        StartCutscene();
    }

    void StartCutscene()
    {
        // Отключаем игрока на время катсцены
        if (player != null) player.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.SetActive(true);

        // Запускаем Timeline
        director.Play();
        director.stopped += OnCutsceneFinished;
    }

    void OnCutsceneFinished(PlayableDirector _)
    {
        // Включаем игрока после катсцены
        if (player != null) player.SetActive(true);
        if (mainCamera != null) mainCamera.SetActive(true);
        if (cutsceneCamera != null) cutsceneCamera.SetActive(false);
    }

    void Update()
    {
        // Пропуск катсцены по Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            director.Stop();
            OnCutsceneFinished(director);
        }
    }
}
