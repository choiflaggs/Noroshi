using UnityEngine;
using System.Collections;

namespace NoroshiDebug.CharacterCapture.Gizmo
{
    public class DrawCaptureGizmo : MonoBehaviour
    {
        // Gizmo側で表示する際にサイズを調整するための値
        // ここは、手動で調整
        const float SCALE_DENOMINATOR = 55.0f;

        Vector3 _thumbnailGizmoSize = new Vector3(116, 116);
        Vector3 _skillButtonGizmoSize = new Vector3(101, 88);
        [SerializeField] Color _thumbnailGizmoColor = Color.white;
        [SerializeField] Color _skillButtonGizmoColor = Color.yellow;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _thumbnailGizmoSize.Set(_thumbnailGizmoSize.x / SCALE_DENOMINATOR,
                                    _thumbnailGizmoSize.y / SCALE_DENOMINATOR,
                                    1.0f);
            _skillButtonGizmoSize.Set(_skillButtonGizmoSize.x / SCALE_DENOMINATOR,
                                      _skillButtonGizmoSize.y / SCALE_DENOMINATOR,
                                      1.0f);
        }

        public void SetThumbnailGizmoWidth(int gizmoWidth)
        {
            _thumbnailGizmoSize.Set(gizmoWidth / SCALE_DENOMINATOR,
                                    _thumbnailGizmoSize.y,
                                    _thumbnailGizmoSize.z);
        }

        public void SetThumbnailGizmoHeight(int gizmoHeight)
        {
            _thumbnailGizmoSize.Set(_thumbnailGizmoSize.x,
                                    gizmoHeight / SCALE_DENOMINATOR,
                                    _thumbnailGizmoSize.z);
        }
        
        public void SetSkillButtonGizmoWidth(int gizmoWidth)
        {
            _skillButtonGizmoSize.Set(gizmoWidth / SCALE_DENOMINATOR,
                                      _skillButtonGizmoSize.y,
                                      _skillButtonGizmoSize.z);
        }
        
        public void SetSkillButtonGizmoHeight(int gizmoHeight)
        {
            _skillButtonGizmoSize.Set(_skillButtonGizmoSize.x,
                                      gizmoHeight / SCALE_DENOMINATOR,
                                      _skillButtonGizmoSize.z);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = _thumbnailGizmoColor;
            Gizmos.DrawWireCube(Vector3.zero, _thumbnailGizmoSize);
            Gizmos.color = _skillButtonGizmoColor;
            Gizmos.DrawWireCube(Vector3.zero, _skillButtonGizmoSize);
        }
    }
}