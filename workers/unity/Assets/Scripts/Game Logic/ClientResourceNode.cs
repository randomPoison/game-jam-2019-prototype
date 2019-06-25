using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    /// <summary>
    /// Client-side logic for displaying and managing resource nodes.
    /// </summary>
    public class ClientResourceNode : MonoBehaviour, IPointerDownHandler
    {
        #region SOS
        [Require]
        private EntityId _entityId = new EntityId();

        [Require]
        private ResourceNodeReader _nodeReader = null;

        // Global state objects.
        private UnityClientConnector _connector;
        private AuthoritativePlayer _player;

        public EntityId EntityId
        {
            get { return _entityId; }
        }

        public ResourceNode.Component Data
        {
            get { return _nodeReader.Data; }
        }
        #endregion

        #region Unity Lifecycle Methods
        private void OnEnable ()
        {
            _connector = FindObjectOfType<UnityClientConnector>();
            _player = FindObjectOfType<AuthoritativePlayer>();
        }
        #endregion

        #region IPointerDownHandler
        public void OnPointerDown (PointerEventData eventData)
        {
            _player.SelectResourceNode(this);
        }
        #endregion
    }
}
