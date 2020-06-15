using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoreMountains.Tools
{	
	[RequireComponent(typeof(Rect))]
	[RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// 将此组件添加到一个GUI Image中，使其充当按钮
    /// 从inspector中给它绑定 pressed down, pressed continually 和relased行为
    /// 控制鼠标和多点触控
    /// </summary>
    public class MMTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, ISubmitHandler
	{
        /// 按钮的不同可能状态：
        /// Off（默认空闲状态）、ButtonDown（第一次按下按钮）、ButtonPressed（按下按钮）、
        /// ButtonUp（松开按钮）、Disabled（无法打开但仍显示在屏幕上）              ButtonDown和ButtonUp只会持续一帧，其他帧则会持续多长时间，您可以按它们/禁用它们/不执行任何操作
        public enum ButtonStates { Off, ButtonEnter , ButtonDown, ButtonPressed, ButtonUp, Disabled }
        [Header("Binding")]
        /// 按钮进入时调用的方法
        public UnityEvent ButtonEntered;
		/// 按钮第一次进入时调用的方法
		public UnityEvent ButtonEnteredFirstTime;
		/// 离开按钮时调用的方法
		public UnityEvent ButtonLeaved;
		/// 按钮按下时调用的方法
		public UnityEvent ButtonPressedFirstTime;
		/// 松开时调用的方法
		public UnityEvent ButtonReleased;
		/// 按住时调用的方法
		public UnityEvent ButtonPressed;
		/// 取消选中时调用
		public UnityEvent ButtonSelectCancel;
		/// 刚进入选中状态时调用
		public UnityEvent ButtonSelectBegain;

        [Header("Sprite Swap")]
        [Information("Here you can define, for disabled and pressed states, if you want a different sprite, and a different color.", InformationAttribute.InformationType.Info, false)]
        public Sprite EnteredSprite;
        public bool EnteredChangeColor = false;
        public Color EnteredColor = Color.white;
        public Sprite DisabledSprite;
		public bool DisabledChangeColor = false;
		public Color DisabledColor = Color.white;
		public Sprite PressedSprite;
		public bool PressedChangeColor = false;
		public Color PressedColor= Color.white;
		public Sprite HighlightedSprite;
		public bool HighlightedChangeColor = false;
		public Color HighlightedColor = Color.white;

		[Header("Opacity")]
		[Information("Here you can set different opacities for the button when it's pressed, idle, or disabled. Useful for visual feedback.",InformationAttribute.InformationType.Info,false)]
        /// 按下按钮时应用于画布组的新不透明度
        public float PressedOpacity = 1f;
		public float IdleOpacity = 1f;
		public float DisabledOpacity = 1f;
        public float EnteredOpacity = 1f;

		[Header("Delays")]
		[Information("Specify here the delays to apply when the button is pressed initially, and when it gets released. Usually you'll keep them at 0.",InformationAttribute.InformationType.Info,false)]
		public float PressedFirstTimeDelay = 0f;
		public float ReleasedDelay = 0f;

		[Header("Buffer")]
		public float BufferDuration = 0f;

		[Header("Animation")]
		[Information("Here you can bind an animator, and specify animation parameter names for the various states.",InformationAttribute.InformationType.Info,false)]
		public Animator Animator;
		public string IdleAnimationParameterName = "Idle";
		public string DisabledAnimationParameterName = "Disabled";
		public string PressedAnimationParameterName = "Pressed";
        public string EnterAnimationParameterName = "Enter";

		[Header("Mouse Mode")]
		[Information("If you set this to true, you'll need to actually press the button for it to be triggered, otherwise a simple hover will trigger it (better to leave it unchecked if you're going for touch input).",InformationAttribute.InformationType.Info,false)]
        /// 如果设置为true，则需要实际按下按钮才能触发它，否则简单的悬停将触发它（更适合于触摸输入）。
        public bool MouseMode = false;

		public bool ReturnToInitialSpriteAutomatically { get; set; }

        /// 当前按钮的状态(off, enter, down, pressed or up)
        public ButtonStates CurrentState ;//{ get; protected set; }

		protected bool _zonePressed = false;
		protected CanvasGroup _canvasGroup;
		protected float _initialOpacity;
		protected Animator _animator;
		protected Image _image;
		protected Sprite _initialSprite;
		protected Color _initialColor;
		protected float _lastClickTimestamp = 0f;
		protected Selectable _selectable;
        protected bool isInArea = false;

        /// <summary>
        /// On Start, 获取canvasgroup，设置初始alpha值
        /// </summary>
        protected virtual void Awake()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			ReturnToInitialSpriteAutomatically = true;

			_selectable = GetComponent<Selectable> ();

			_image = GetComponent<Image> ();
			if (_image != null)
			{
				_initialColor = _image.color;
				_initialSprite = _image.sprite;
			}

			_animator = GetComponent<Animator>();
			if (Animator != null)
			{
				_animator = Animator;
			}

			_canvasGroup = GetComponent<CanvasGroup>();
			if (_canvasGroup!=null)
			{
				_initialOpacity = IdleOpacity;
				_canvasGroup.alpha = _initialOpacity;
				_initialOpacity = _canvasGroup.alpha;
			}
			ResetButton();
		}

		/// <summary>
        /// 每一帧，如果触摸区域被按下，就触发OnPointerPressed
		/// </summary>
		protected virtual void Update()
		{
			switch (CurrentState)
			{
				case ButtonStates.Off:
					SetOpacity (IdleOpacity);
					if ((_image != null) && (ReturnToInitialSpriteAutomatically))
					{
						_image.color = _initialColor;
						_image.sprite = _initialSprite;
					}
					if (_selectable != null)
					{
						_selectable.interactable = true;
						if (EventSystem.current.currentSelectedGameObject == this.gameObject)
						{
							if ((_image != null) && HighlightedChangeColor)
							{
								_image.color = HighlightedColor;
							}
							if (HighlightedSprite != null)
							{
								_image.sprite = HighlightedSprite;
							}
						}
					}
					break;

                case ButtonStates.ButtonEnter:
                    SetOpacity(EnteredOpacity);
                    if (ButtonEntered != null)
                    {
                        ButtonEntered.Invoke();
                    }
                    if (_image != null)
                    {
                        if (EnteredSprite != null)
                        {
                            _image.sprite = EnteredSprite;
                        }
                        if (EnteredChangeColor)
                        {
                            _image.color = EnteredColor;
                        }
                    }
                    if (_selectable != null)
                    {
                        _selectable.interactable = true;
                    }
                    break;

                case ButtonStates.Disabled:
					SetOpacity (DisabledOpacity);
					if (_image != null)
					{
						if (DisabledSprite != null)
						{
							_image.sprite = DisabledSprite;	
						}
						if (DisabledChangeColor)
						{
							_image.color = DisabledColor;	
						}
					}
					if (_selectable != null)
					{
						_selectable.interactable = false;
					}
					break;

				case ButtonStates.ButtonDown:

					break;

				case ButtonStates.ButtonPressed:
					SetOpacity (PressedOpacity);
					OnPointerPressed();
					if (_image != null)
					{
						if (PressedSprite != null)
						{
							_image.sprite = PressedSprite;
						}
						if (PressedChangeColor)
						{
							_image.color = PressedColor;	
						}
					}
					break;

				case ButtonStates.ButtonUp:

					break;
			}
			UpdateAnimatorStates ();
		}

        /// <summary>
        /// 在每帧结束时，如果需要，我们会更改按钮的状态
        /// </summary>
        protected virtual void LateUpdate()
		{
			if (CurrentState == ButtonStates.ButtonUp)
            {
                if (!isInArea)
                {
                    CurrentState = ButtonStates.Off;
                }
                else
				    CurrentState = ButtonStates.ButtonEnter;
			}

			if (CurrentState == ButtonStates.ButtonDown)
			{
				CurrentState = ButtonStates.ButtonPressed;
			}

            if (CurrentState == ButtonStates.Off && isInArea)
            {
				CurrentState = ButtonStates.ButtonEnter;
				ButtonEnteredFirstTime?.Invoke();
			}
		}

        /// <summary>
        /// 触发绑定指针向下操作
        /// </summary>
        public virtual void OnPointerDown(PointerEventData data)
		{
			if (Time.time - _lastClickTimestamp < BufferDuration)
			{
				return;
			}

			if (CurrentState != ButtonStates.Off && CurrentState != ButtonStates.ButtonEnter)
			{
				return;
			}
			CurrentState = ButtonStates.ButtonDown;
			_lastClickTimestamp = Time.time;
			if ((Time.timeScale != 0) && (PressedFirstTimeDelay > 0))
			{
				Invoke ("InvokePressedFirstTime", PressedFirstTimeDelay);	
			}
			else
			{
				ButtonPressedFirstTime.Invoke();
			}
		}

		protected virtual void InvokePressedFirstTime()
		{
			if (ButtonPressedFirstTime!=null)
			{
				ButtonPressedFirstTime.Invoke();
			}
		}

        /// <summary>
        /// 触发绑定指针向上操作
        /// </summary>
        public virtual void OnPointerUp(PointerEventData data)
		{
			if (CurrentState != ButtonStates.ButtonPressed && CurrentState != ButtonStates.ButtonDown)
			{
				return;
			}

			CurrentState = ButtonStates.ButtonUp;
			if ((Time.timeScale != 0) && (ReleasedDelay > 0))
			{
				Invoke ("InvokeReleased", ReleasedDelay);
			}
			else
			{
				ButtonReleased.Invoke();
			}
		}

		protected virtual void InvokeReleased()
		{
			if (ButtonReleased != null)
			{
				ButtonReleased.Invoke();
			}			
		}

        /// <summary>
        /// 触发绑定指针按压操作
        /// </summary>
        public virtual void OnPointerPressed()
		{
			CurrentState = ButtonStates.ButtonPressed;
			if (ButtonPressed != null)
			{
				ButtonPressed.Invoke();
			}
		}

		/// <summary>
		/// 重置按钮状态和透明度
		/// </summary>
		protected virtual void ResetButton()
		{
			SetOpacity(_initialOpacity);
			CurrentState = ButtonStates.Off;
			isInArea = false;
		}

        /// <summary>
        /// 当触摸进入区域时触发绑定指针进入操作
        /// </summary>
        public virtual void OnPointerEnter(PointerEventData data)
		{
			if (!MouseMode)
			{
				OnPointerDown (data);
			}
            else
            {
                if (CurrentState == ButtonStates.Off)
                {
                    CurrentState = ButtonStates.ButtonEnter;
					ButtonEnteredFirstTime?.Invoke();
				}
			}
            isInArea = true;
        }

        /// <summary>
        /// 当触摸进入区域时触发绑定指针退出操作
        /// </summary>
        public virtual void OnPointerExit(PointerEventData data)
		{
			if (!MouseMode)
			{
				OnPointerUp(data);	
			}
            else
            {
                if (CurrentState == ButtonStates.ButtonEnter)
                {
                    CurrentState = ButtonStates.Off;
					ButtonLeaved.Invoke();
                }
            }
            isInArea = false;
        }
        /// <summary>
        /// OnEnable, 重置按钮状态
        /// </summary>
        protected virtual void OnEnable()
		{
			ResetButton();
		}

		public virtual void DisableButton()
		{
			CurrentState = ButtonStates.Disabled;
		}

		public virtual void EnableButton()
		{
			if (CurrentState == ButtonStates.Disabled)
			{
				CurrentState = ButtonStates.Off;	
			}
		}

		protected virtual void SetOpacity(float newOpacity)
		{
			if (_canvasGroup!=null)
			{
				_canvasGroup.alpha = newOpacity;
			}
		}

		protected virtual void UpdateAnimatorStates ()
		{
			if (_animator == null)
			{
				return;
			}
			if (DisabledAnimationParameterName != null)
			{
				_animator.SetBool (DisabledAnimationParameterName, (CurrentState == ButtonStates.Disabled));
			}
			if (PressedAnimationParameterName != null)
			{
				_animator.SetBool (PressedAnimationParameterName, (CurrentState == ButtonStates.ButtonPressed));
			}
			if (IdleAnimationParameterName != null)
			{
				_animator.SetBool (IdleAnimationParameterName, (CurrentState == ButtonStates.Off));
			}
            if (EnterAnimationParameterName != null)
            {
                _animator.SetBool(EnterAnimationParameterName, (CurrentState == ButtonStates.ButtonEnter));
            }
        }

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (ButtonPressedFirstTime!=null)
			{
				ButtonPressedFirstTime.Invoke();
			}
			if (ButtonReleased!=null)
			{
				ButtonReleased.Invoke ();
			}
		}
	}
}