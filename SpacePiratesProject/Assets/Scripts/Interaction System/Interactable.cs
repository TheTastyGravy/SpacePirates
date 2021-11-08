using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Interactable : MonoBehaviour
{
	public InteractionPromptLogic interactionPrompt;
	public float interactionRadius = 1;
	[SerializeField]
	private Vector3 interactionOffset;

	public Vector3 InteractionCenter { get => transform.position + transform.rotation * interactionOffset; }

	protected Interactor currentInteractor = null;
	public bool IsBeingUsed => currentInteractor != null;



	protected virtual void Awake()
	{
		InteractionManager.Instance.interactables.Add(this);
		if (interactionPrompt != null)
			interactionPrompt.Interactable = this;
	}

	protected virtual void OnDestroy()
	{
		if (InteractionManager.Instance != null)
			InteractionManager.Instance.interactables.Remove(this);
	}

	protected virtual void OnEnable()
	{
		if (interactionPrompt != null)
			interactionPrompt.enabled = true;
	}

	protected virtual void OnDisable()
	{
		if (interactionPrompt != null)
			interactionPrompt.enabled = false;
	}

	/// <summary>
	/// Returns true if the interactor can interact with the control
	/// </summary>
	internal bool CanBeUsed(Interactor interactor, Player.Control control)
	{
		return isActiveAndEnabled && !IsBeingUsed && CanBeUsed(interactor, out Player.Control outControl) && outControl == control;
	}

	/// <summary>
	/// Can we be interacted with by the interactor?
	/// </summary>
	/// <param name="button">The button that the interactor will use to interact</param>
	protected virtual bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
		// Basic implementation
		button = Player.Control.A_PRESSED;
		return true;
	}

	internal void StartInteraction(Interactor interactor)
	{
		if (IsBeingUsed)
			return;

		if (interactionPrompt != null)
			interactionPrompt.InteractStart();

		currentInteractor = interactor;
		OnInteractionStart();
		OnButtonDown();
	}

	internal void StopInteraction(Interactor interactor)
	{
		if (!IsBeingUsed || interactor != currentInteractor)
			return;

		OnInteractionStop();
		currentInteractor = null;

		if (interactionPrompt != null)
			interactionPrompt.InteractStop();
	}

	internal void ButtonDown(Interactor interactor)
	{
		if (!IsBeingUsed || interactor != currentInteractor)
			return;

		OnButtonDown();
	}

	internal void ButtonUp(Interactor interactor)
	{
		if (!IsBeingUsed || interactor != currentInteractor)
			return;

		OnButtonUp();
	}

	protected virtual void OnInteractionStart() { }
	protected virtual void OnInteractionStop() { }
	protected virtual void OnButtonDown() { }
	protected virtual void OnButtonUp() { }

#if UNITY_EDITOR
	[DrawGizmo(GizmoType.InSelectionHierarchy, typeof(Interactable))]
	private static void DrawGizmos(Interactable interactable, GizmoType gizmoType)
	{
		if (!interactable.enabled)
			return;

		// Setup rotation matrix to rotate the sphere without affecting its position
		Handles.matrix = Matrix4x4.Rotate(interactable.transform.rotation);
		Vector3 position = Handles.inverseMatrix.MultiplyPoint(interactable.InteractionCenter);

		// Set values used for outer disc depending on camera
		Vector3 normal;
		float sqrMagnitude, num0, num1;
		if (Camera.current.orthographic)
		{
			normal = -Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
			sqrMagnitude = 1;
			num0 = interactable.interactionRadius * interactable.interactionRadius;
			num1 = interactable.interactionRadius;
		}
		else
		{
			normal = position - Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
			sqrMagnitude = normal.sqrMagnitude;
			num0 = interactable.interactionRadius * interactable.interactionRadius;
			num1 = Mathf.Sqrt(num0 - (num0 * num0 / sqrMagnitude));
		}


		void DrawSphere()
		{
			Handles.DrawWireDisc(position, Vector3.right, interactable.interactionRadius);
			Handles.DrawWireDisc(position, Vector3.up, interactable.interactionRadius);
			Handles.DrawWireDisc(position, Vector3.forward, interactable.interactionRadius);
			Handles.DrawWireDisc(position - num0 * normal / sqrMagnitude, normal, num1);
		}
		// First pass: lines in front get normal alpha
		Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
		Handles.color = new Color(1, 1, 0.5f, 1);
		DrawSphere();
		// Second pass: lines behind get lower alpha
		Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
		Handles.color = new Color(1, 1, 0.5f, 0.1f);
		DrawSphere();
	}
#endif
}
