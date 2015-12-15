using System;
using UnityEngine;

namespace NoroshiDebug.BattleResultUI
{
    [SerializableAttribute]
    public class SerializableCharacterThumbnail
    {
        [SerializeField]
        public uint CharacterID;
        [SerializeField]
        public ushort Level;
        [SerializeField]
        public byte EvolutionLevel;
        [SerializeField]
        public byte PromotionLevel;
        [SerializeField]
        public bool IsDead;
        [SerializeField]
        public byte SkinLevel;
    }
}