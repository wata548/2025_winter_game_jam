using UnityEngine;

public class KeyboardButtonHover : MonoBehaviour
{
    [Header("버튼 오브젝트 배열")]
    [SerializeField] private Transform[] buttons;

    [Header("버튼 커질 크기")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);

    [Header("스케일 애니메이션 속도")]
    [SerializeField] private float lerpSpeed = 10f;

    private Vector3[] originalScales;
    private int currentIndex = 0;  // 선택된 버튼 인덱스

    private void Awake()
    {
        // 각 버튼 원래 크기 저장
        originalScales = new Vector3[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            originalScales[i] = buttons[i].localScale;
        }

        // 시작할 때는 모든 버튼 원래 크기 유지
        // buttons[currentIndex].localScale = hoverScale;  ← 제거
    }

    private void Update()
    {
        // 키 입력 체크
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeButton(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeButton(1);
        }

        // 버튼 스케일 부드럽게 적용
        for (int i = 0; i < buttons.Length; i++)
        {
            Vector3 target = (i == currentIndex) ? hoverScale : originalScales[i];
            buttons[i].localScale = Vector3.Lerp(buttons[i].localScale, target, Time.deltaTime * lerpSpeed);
        }
    }

    private void ChangeButton(int direction)
    {
        // 순환 처리
        currentIndex += direction;
        if (currentIndex < 0)
            currentIndex = buttons.Length - 1;
        else if (currentIndex >= buttons.Length)
            currentIndex = 0;
    }
}
