using System;
using UniLinq;

namespace Noroshi.BattleScene.Actions
{
    public class ActionSequence
    {
        sbyte[] _actionSequences;
        int _secondLoopStartPosition;
        int _currentNo;
        int _loopNo;
        public ActionSequence(Core.WebApi.Response.Character.CharacterActionSequence data)
        {
            _secondLoopStartPosition = data.SecondLoopStartPosition;
             _actionSequences = new sbyte[7]{
                data.ActionSequence1,
                data.ActionSequence2,
                data.ActionSequence3,
                data.ActionSequence4,
                data.ActionSequence5,
                data.ActionSequence6,
                data.ActionSequence7,
            };
            _loopNo = 1;
            // 初回位置を決める
            _currentNo = 0;
            Switch();
        }
        public byte GetRank()
        {
            return (byte)_getActionSequences(_currentNo);
        }

        public void Switch()
        {
            ++_currentNo;
            if (_currentNo > _actionSequences.Length)
            {
                if (_loopNo == 1)
                {
                    _currentNo = _secondLoopStartPosition;
                }
                else
                {
                    _currentNo = 1;
                }
                ++_loopNo;
            }
            if (_getActionSequences(_currentNo) < 0)
            {
                Switch();
            }
        }

        sbyte _getActionSequences(int no)
        {
            return _actionSequences[no - 1];
        }
    }
}