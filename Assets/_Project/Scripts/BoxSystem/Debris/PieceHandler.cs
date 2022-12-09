using UnityEngine;
using System.Collections.Generic;
using ZestCore.Utility;

namespace DigFight
{
    public class PieceHandler : MonoBehaviour
    {
        private List<Piece> _pieces = new List<Piece>();
        private int _pieceCount;

        #region PROPERTIES
        public BreakableBox BreakableBox { get; private set; }
        #endregion

        public void Init(BreakableBox breakableBox)
        {
            BreakableBox = breakableBox;

            InitializePieces();
        }

        #region HELPERS
        private void InitializePieces()
        {
            foreach (Piece piece in GetComponentsInChildren<Piece>())
            {
                AddPiece(piece);
                piece.Init(this);
            }
            _pieceCount = _pieces.Count;
            RNG.ShuffleList(_pieces);
        }
        private void AddPiece(Piece piece)
        {
            if (!_pieces.Contains(piece))
                _pieces.Add(piece);
        }
        private void RemovePiece(Piece piece)
        {
            if (_pieces.Contains(piece))
            {
                _pieces.Remove(piece);
                _pieceCount--;
            }
        }
        #endregion

        #region PUBLICS
        public void Release()
        {
            if (_pieceCount <= 0) return;

            int releaseCount = (int)(_pieceCount * BreakableBox.GetCurrentHealthNormalized());

            if (releaseCount > _pieceCount)
                releaseCount = _pieceCount;
            
            for (int i = 0; i < releaseCount; i++)
            {
                _pieces[0].StartPullOutSequence();
                RemovePiece(_pieces[0]);
            }
        }
        #endregion
    }
}
