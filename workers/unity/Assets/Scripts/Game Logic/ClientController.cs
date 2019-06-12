﻿using System;
using Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    public class ClientController : MonoBehaviourSingleton<ClientController>
    {
        private MainCanvasController _canvas = null;

        private ClientDrone _selectedDrone = null;

        public event Action<ClientDrone> SelectedDroneChanged;

        public void SelectDrone(ClientDrone drone)
        {
            _selectedDrone = drone;
            _canvas.SetActiveDrone(drone);

            SelectedDroneChanged?.Invoke(drone);
        }

        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();

            // Initialize references to objects in the scene.
            _canvas = FindObjectOfType<MainCanvasController>();
        }
        #endregion
    }
}
