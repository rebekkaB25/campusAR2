﻿//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Allows a long tap and hold to move an object around and release it at a new point. Add this script to the object to drag.
    /// </summary>
    [AddComponentMenu("Fingers Gestures/Component/Drag and Drop", 0)]
    public class FingersDragDropComponentScript : MonoBehaviour
    {
        [Tooltip("The camera to use to convert screen coordinates to world coordinates. Defaults to Camera.main.")]
        public Camera Camera;

        [Tooltip("Whether to bring the object to the front when a gesture executes on it")]
        public bool BringToFront = true;

        private LongPressGestureRecognizer longPressGesture;
        private Rigidbody2D rigidBody;
        private SpriteRenderer spriteRenderer;
        private int startSortOrder;
        private float panZ;
        private Vector3 panOffset;

        private void LongPressGestureUpdated(GestureRecognizer r)
        {
            FingersPanRotateScaleComponentScript.StartOrResetGesture(r, BringToFront, Camera, gameObject, spriteRenderer, GestureRecognizerComponentScriptBase.GestureObjectMode.RequireIntersectWithGameObject);
            if (r.State == GestureRecognizerState.Began)
            {
                panZ = Camera.WorldToScreenPoint(transform.position).z;
                panOffset = transform.position - Camera.ScreenToWorldPoint(new Vector3(r.FocusX, r.FocusY, panZ));
                if (DragStarted != null)
                {
                    DragStarted.Invoke(this, System.EventArgs.Empty);
                }
            }
            if (r.State == GestureRecognizerState.Executing)
            {
                Vector3 gestureScreenPoint = new Vector3(r.FocusX, r.FocusY, panZ);
                Vector3 gestureWorldPoint = Camera.ScreenToWorldPoint(gestureScreenPoint) + panOffset;
                if (rigidBody == null)
                {
                    transform.position = gestureWorldPoint;
                }
                else
                {
                    rigidBody.MovePosition(gestureWorldPoint);
                }
                if (DragUpdated != null)
                {
                    DragUpdated.Invoke(this, System.EventArgs.Empty);
                }
            }
            else if (r.State == GestureRecognizerState.Ended)
            {
                if (spriteRenderer != null && BringToFront)
                {
                    spriteRenderer.sortingOrder = startSortOrder;
                }
                if (DragEnded != null)
                {
                    DragEnded.Invoke(this, System.EventArgs.Empty);
                }
            }
        }

        private void Start()
        {
            this.Camera = (this.Camera == null ? Camera.main : this.Camera);
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.StateUpdated += LongPressGestureUpdated;
            rigidBody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                startSortOrder = spriteRenderer.sortingOrder;
            }
            FingersScript.Instance.AddGesture(longPressGesture);
        }

        private void Update()
        {

        }

        public event System.EventHandler DragStarted;
        public event System.EventHandler DragUpdated;
        public event System.EventHandler DragEnded;
    }
}
