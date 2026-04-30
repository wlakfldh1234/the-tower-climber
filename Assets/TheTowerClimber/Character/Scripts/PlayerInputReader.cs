using System;
using UnityEngine;
using UnityEngine.InputSystem;

//유니티 매크로 기능 
//개발자 실수 방지 - 개발적 단단함 
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    public PlayerInput playerInput;
    //PlayerInput:: Unity new Input system 의 관련 주요 클래스

    public InputAction moveAction;
    public InputAction toggleStatusAction;
    public InputAction jumpAction;
    public InputAction pauseAction;

    public Vector2 MoveVector { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool ToggleStatusPressedThisFrame { get; private set; }
    public bool PausePressedThisFrame { get; private set; }

    [Header("Action Names")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string pauseActionName = "Pause";
    [SerializeField] private string toggleStatusActionName = "StatusToggle";

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        ResolveActions();
        EnableAllActions();
    }

    private void OnEnable() => EnableAllActions(); // 씬 재진입 시에도 보장

    private void OnDisable() => DisableAllActions();

    private void EnableAllActions()
    {
        moveAction?.Enable();
        toggleStatusAction?.Enable();
        jumpAction?.Enable();
        pauseAction?.Enable();
    }

    private void DisableAllActions()
    {
        moveAction?.Disable();
        toggleStatusAction?.Disable();
        jumpAction?.Disable();
        pauseAction?.Disable();
    }

    //update 최대한 아껴야하는, 게임적인 매프레임 반응속도를 원하는 구현 X 
    // 입력 관련 구현은 Update 적합 
    private void Update()
    {
        MoveVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        ToggleStatusPressedThisFrame = toggleStatusAction != null && toggleStatusAction.WasPerformedThisFrame();
        JumpPressedThisFrame = jumpAction != null && jumpAction.WasPerformedThisFrame();
        PausePressedThisFrame = pauseAction != null && pauseAction.WasPerformedThisFrame();
    }

    private void ResolveActions()
    {
        //방어코드
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.Log("[PlayerInputReader] PlayerInput 또는 Action Asset 확인 필요");
            return;
        }

        moveAction = FindAction(moveActionName);
        jumpAction = FindAction(jumpActionName);
        pauseAction = FindAction(pauseActionName);
        toggleStatusAction = FindAction(toggleStatusActionName);
    }

    private InputAction FindAction(string actionName)
    {
        //방어코드 
        if (string.IsNullOrWhiteSpace(actionName)) return null;

        InputAction action = playerInput.actions.FindAction(actionName, false);
        if (action == null)
            Debug.LogWarning($"[PlayerInputReader] Action 못 찾음: {actionName} ");
        return action;
    }
}
