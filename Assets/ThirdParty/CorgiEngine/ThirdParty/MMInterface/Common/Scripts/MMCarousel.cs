using UnityEngine;
using System.Collections;
using System;
using MoreMountains.Tools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MoreMountains.MMInterface
{
    /// <summary>
    /// 用于处理放置在HorizontalLayoutGroup中的UI元素传送带的类。
    /// 传送带中所有元素都要有相同的宽度.
    /// </summary>
    public class MMCarousel : MonoBehaviour
	{
		[Header("Binding")]
		/// 包含所有UI元素的layout group
		public HorizontalLayoutGroup Content;

		public Camera UICamera;

		[Header("Optional Buttons Binding")]
		/// 让传送带向左移动的按钮
		public MMTouchButton LeftButton;
		/// 让传送带向右移动的按钮
		public MMTouchButton RightButton;

		[Header("Carousel Setup")]
		/// 初始和当前的索引
		public int CurrentIndex = 0;
		/// 传送带每次移动的物品数量
		public int Pagination = 1;
        /// 到达时将停止移动的距离百分比
        public float ThresholdInPercent = 1f;

		[Header("Speed")]
		/// 传送带移动过程的持续时间（秒） 
		public float MoveDuration = 0.05f;

		[Header("Focus")]
		/// 最初显示的物品
		public GameObject InitialFocus;



		protected float _elementWidth;
		protected int _contentLength = 0;
		protected float _spacing;
		protected Vector2 _initialPosition;
		protected RectTransform _rectTransform;

		protected bool _lerping = false;
		protected float _lerpStartedTimestamp;
		protected Vector2 _startPosition;
		protected Vector2 _targetPosition;

		/// <summary>
		/// 开始时初始化传送带
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
        /// 初始化传送带，获取rect transform，计算元素维度， 初始化位置
		/// </summary>
		protected virtual void Initialization()
		{
			_rectTransform = Content.gameObject.GetComponent<RectTransform> ();
			_initialPosition = _rectTransform.anchoredPosition;

			// 计算内容的元素宽度
			_contentLength = 0;
			foreach (Transform tr in Content.transform) 
			{ 
				_elementWidth = tr.gameObject.GetComponentNoAlloc<RectTransform>().sizeDelta.x;
				_contentLength++;
			}
			_spacing = Content.spacing;

			// 把传送带位置改到预期的初始索引处
			_rectTransform.anchoredPosition = DeterminePosition ();

			if (InitialFocus != null)
			{
				EventSystem.current.SetSelectedGameObject(InitialFocus, null);
			}

		}

		/// <summary>
		/// 向左移动传送带
		/// </summary>
		public virtual void MoveLeft()
		{
			if (!CanMoveLeft())
			{
				return;
			}
			else
			{				
				CurrentIndex -= Pagination;
				MoveToCurrentIndex ();	
			}
		}

		/// <summary>
		/// 向右移动传送带
		/// </summary>
		public virtual void MoveRight()
		{
			if (!CanMoveRight())
			{
				return;
			}
			else
			{
				CurrentIndex += Pagination;
				MoveToCurrentIndex ();	
			}
		}

		/// <summary>
		/// 启动对当前索引的移动
		/// </summary>
		protected virtual void MoveToCurrentIndex ()
		{
			_startPosition = _rectTransform.anchoredPosition;
			_targetPosition = DeterminePosition ();
			_lerping = true;
			_lerpStartedTimestamp = Time.time;
		}

		/// <summary>
		/// 根据当前索引值决定目标位置
		/// </summary>
		/// <returns>The position.</returns>
		protected virtual Vector2 DeterminePosition()
		{
			return _initialPosition - (Vector2.right * CurrentIndex * (_elementWidth + _spacing));
		}

		public virtual bool CanMoveLeft()
		{
			return (CurrentIndex - Pagination >= 0);
				
		}

		/// <summary>
		/// 是否传送带可以向右移动
		/// </summary>
		/// <returns><c>true</c> 如果可以向右移动; 否则, <c>false</c>.</returns>
		public virtual bool CanMoveRight()
		{
			return (CurrentIndex + Pagination < _contentLength);
		}

		/// <summary>
		/// On Update 如果需要，则移动传送带，并处理按钮的状态
		/// </summary>
		protected virtual void Update()
		{
			if (_lerping)
			{
				LerpPosition ();
			}
			HandleButtons ();
			HandleFocus ();
		}

		protected virtual void HandleFocus()
		{
			if (!_lerping && Time.timeSinceLevelLoad > 0.5f)
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					if (UICamera.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position).x < 0)
					{
						MoveLeft ();
					}
					if (UICamera.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position).x > Screen.width)
					{
						MoveRight ();
					}	
				}
			}
		}

        /// <summary>
        /// 处理按钮，必要时启用和禁用它们
        /// </summary>
        protected virtual void HandleButtons()
		{
			if (LeftButton != null) 
			{ 
				if (CanMoveLeft())
				{
					LeftButton.EnableButton (); 
				}
				else
				{
					LeftButton.DisableButton (); 
				}	
			}
			if (RightButton != null) 
			{ 
				if (CanMoveRight())
				{
					RightButton.EnableButton (); 
				}
				else
				{
					RightButton.DisableButton (); 
				}	
			}
		}

		/// <summary>
		/// 缓动传送带的位置
		/// </summary>
		protected virtual void LerpPosition()
		{
			float timeSinceStarted = Time.time - _lerpStartedTimestamp;
			float percentageComplete = timeSinceStarted / MoveDuration;

			_rectTransform.anchoredPosition = Vector2.Lerp (_startPosition, _targetPosition, percentageComplete);

            //当完成缓动时，设置_isLerping为false
			if(percentageComplete >= ThresholdInPercent)
			{
				_lerping = false;
			}
		}
	}
}