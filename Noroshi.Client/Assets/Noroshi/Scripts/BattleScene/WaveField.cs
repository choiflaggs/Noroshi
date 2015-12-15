using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Vector2 = UnityEngine.Vector2;
using Noroshi.Core.Game.Character;
using Noroshi.Grid;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene
{
    /// フィールドにおける各キャラクターの位置情報（グリッド座標系、ワールド座標系）を扱うクラス。
    /// あくまでも位置情報を扱うだけなので内包するキャラクター自体のライフサイクル管理は関知しない。
    /// 内包するキャラクターの View 移動も担当するが、アニメーションなどは扱わない。あくまでもグリッド座標系に紐付くワールド座標系の位置移動のみ。
    /// （一時的に）利用が終了したら Clear して内容するキャラクターへの参照を切っておくこと。
    public class WaveField : IActionTargetFinder
    {
         /// 水平方向のフィールドサイズ（ワールド座標系）。
        public const float HORIZONTAL_LENGTH = 12.80f;
        /// 垂直方向のフィールドサイズ（ワールド座標系）。
        const float VERTICAL_LENGTH   =  2.4f;
        /// 垂直方向のフィールド座標（ワールド座標系）。
        const float VERTICAL_POSITION =  -1.4f;
        /// 同一水平方向グリッド位置であっても垂直方向グリッドが異なれば水平ワールド座標系をずらす。そのずらし幅。
        const float HORIZONTAL_LENGTH_DIFF = 0.01f;

        /// 水平方向のフィールド座標（ワールド座標系）を取得する。
        public static float GetPositionX(byte waveNo)
        {
            return HORIZONTAL_LENGTH * (waveNo - 1);
        }

        /// グリッド座標系のコンテナ。
        GridContainer<Character> _gridContainer;
        /// グリッド座標計算機。
        PositionCalculator _positionCalculator = new PositionCalculator();
        
        /// ウェーブ番号。
        public readonly byte WaveNo;

        /// ウェーブ番号指定でインスタンス化。
        public WaveField(byte waveNo)
        {
            WaveNo = waveNo;
            _gridContainer = new GridContainer<Character>(Constant.FIELD_HORIZONTAL_GRID_SIZE, _positionCalculator.GetVerticalSize());
        }
        /// 内包物クリア（Disponse は不要）。
        public void Clear()
        {
            _gridContainer.RemoveAllContents();
        }

        /// 水平方向のグリッドサイズ。
        public ushort HorizontalSize { get { return _gridContainer.HorizontalSize; } }
        /// 垂直方向のグリッドサイズ。
        public ushort VerticalSize { get { return _gridContainer.VerticalSize  ; } }

        /// キャラクターを配置する。
        public void SetCharacter(ushort h, ushort v, Character character)
        {
            _gridContainer.SetContent(h, v, character);
        }
        /// キャラクターを移動する。
        public void MoveCharacter(Character character, GridPosition gridPosition)
        {
            _gridContainer.MoveTo(character, gridPosition.HorizontalIndex, gridPosition.VerticalIndex);
        }
        /// 方向指定でキャラクターを移動する。
        public void MoveCharacter(Character character, Direction direction)
        {
            _gridContainer.MoveTo(character, direction);
        }
        /// キャラクターを View とともに開始位置へ移動する。
        public void MoveCharacterToAvailableStartPositionWithView(Character character, ICharacterView characterView)
        {
            MoveCharacter(character, new GridPosition(GetAvailableStartHorizontalIndex(character.Force), character.GetGridPosition().Value.VerticalIndex));
            characterView.SetPosition(GetPosition(character.GetGridPosition().Value));
        }
        /// View 移動を伴いつつ、キャラクターを水平方法へ徐々に移動する。
        public IObservable<GridPosition> MoveCharacterHorizontallyWithView(Character character, ICharacterView characterView, Direction direction, short horizontalDiff, float duration)
        {
            var sign = _getHorizontalSign(direction, horizontalDiff);
            var positions = _getViewPositionsToMove(character, direction, horizontalDiff);
            return characterView.Move(positions, duration)
            .Select(_ =>
            {
                var nextGridPosition = character.GetGridPosition().Value.BuildNextGrid(direction, sign);
                MoveCharacter(character, nextGridPosition);
                return nextGridPosition;
            });
        }
        /// View 移動を伴いつつ、キャラクターを水平方法へ徐々に移動する。移動差分は毎回計算する方式。
        public IObservable<GridPosition> MoveCharacterHorizontallyWithView(Character character, ICharacterView characterView, Direction direction, Func<short> getHorizontalDiff, float duration)
        {
            var startGridPosition = character.GetGridPosition().Value;
            var sign = _getHorizontalSign(direction, getHorizontalDiff());
            var positions = _getViewPositionsToMove(character, direction, getHorizontalDiff());
            return characterView.Move(positions, duration)
            .Select(_ =>
            {
                var currentGridPosition = character.GetGridPosition().Value;
                var canMove = direction == Direction.Right
                    ? currentGridPosition.HorizontalIndex < startGridPosition.BuildNextGrid(direction, getHorizontalDiff()).HorizontalIndex
                    : currentGridPosition.HorizontalIndex > startGridPosition.BuildNextGrid(direction, getHorizontalDiff()).HorizontalIndex;
                if (!canMove)
                {
                    characterView.StopMove();
                    return currentGridPosition;
                }
                var nextGridPosition = character.GetGridPosition().Value.BuildNextGrid(direction, sign);
                MoveCharacter(character, nextGridPosition);
                return nextGridPosition;
            });
        }
        /// View 移動を伴いつつ、キャラクターを水平方法の終端へ徐々に移動する。
        public IObservable<GridPosition> MoveCharacterToHorizontalEndWithView(Character character, ICharacterView characterView, Direction direction, float duration)
        {
            var horizontalDiff = (short)(GetEndHorizontalIndex(direction) - character.GetGridPosition().Value.HorizontalIndex);
            return MoveCharacterHorizontallyWithView(character, characterView, direction, horizontalDiff, duration);
        }
        Vector2[] _getViewPositionsToMove(Character character, Direction direction, short horizontalDiff)
        {
            var horizontalDiffAbs = Math.Abs(horizontalDiff);
            var sign = _getHorizontalSign(direction, horizontalDiff);
            var initialCharacterGridPosition = character.GetGridPosition().Value;
            var positions = new Vector2[horizontalDiffAbs];
            for (var h = 1; h <= horizontalDiffAbs; h++)
            {
                positions[h - 1] = GetPosition(new GridPosition((ushort)(initialCharacterGridPosition.HorizontalIndex + h * sign), initialCharacterGridPosition.VerticalIndex));
            }
            return positions;
        }
        int _getHorizontalSign(Direction direction, short horizontalDiff)
        {
            var horizontalDiffAbs = Math.Abs(horizontalDiff);
            horizontalDiff = direction == Direction.Right ? horizontalDiff : (short)-horizontalDiff;
            return horizontalDiff / horizontalDiffAbs;            
        }

        /// キャラクターを取り除く。
        public void RemoveCharacter(Character character)
        {
            _gridContainer.RemoveContent(character);
        }
        /// 配置されているキャラクターを全て取得する。
        public List<Character> GetAllCharacters()
        {
            return _gridContainer.GetContents();
        }

        /// 対象キャラクターを前に進めるべきかどうかを判定する。
        /// 進行方向 1 ~ 最大基本アクション レンジ内に敵がいなければ進むべきと判定。
        public bool ShouldMoveForward(Character character, Direction proceedDirection, ushort baseActionRange)
        {
            return !GetTargetsWithHorizontalRange(character.GetGridPosition().Value, proceedDirection, 1, baseActionRange)
                .Any(target => target.Force != character.CurrentForce);
        }
        /// 変更すべき進行方向があれば取得する。null だと現在の方向のままで問題ないという判定。
        public Direction? GetProceedDirectionToChange(Character character)
        {
            // 距離が近い敵キャラクターを選出。
            var target = GetAllCharacters()
                .Where(c => c.Force != character.CurrentForce && c != character)
                .OrderBy(c => _gridContainer.GetHorizontalDistance(character, c))
                .FirstOrDefault();
            if (target == null) return null;
            var direction = _gridContainer.GetHorizontalDirection(character, target);
            if (direction.HasValue && direction.Value != character.GetDirection())
            {
                return direction.Value;
            }
            return null;
        }

        /// 対象実行者をターゲットとして解釈する。
        /// IActionTargetFinder 実装メソッド。
        public IActionTarget GetExecutorAsTarget(IActionExecutor executor)
        {
            return (Character)executor;
        }
        /// 全てのアクション対象を取得する。
        /// IActionTargetFinder 実装メソッド。
        public IEnumerable<IActionTarget> GetAllTargets()
        {
            return GetAllCharacters().Where(at => at.IsTargetable).Cast<IActionTarget>();
        }
        /// レンジ指定でアクション対象を取得する。
        /// IActionTargetFinder 実装メソッド。
        public IEnumerable<IActionTarget> GetTargetsWithHorizontalRange(GridPosition baseGridPosition, Direction horizontalDirection, int minRange, int maxRange)
        {
            return _gridContainer.GetContentsByHorizontalRange(baseGridPosition.HorizontalIndex, horizontalDirection, minRange, maxRange).Select(c => (IActionTarget)c);
        }

        /// 自キャラクターを初期配置する。
        public void SetOwnCharactersToInitialGrid(IEnumerable<Character> characters)
        {
            _setCharactersToInitialGrid(
                characters,
                (character, no, characterNum) =>
                {
                    if (character.IsFirstHidden) return 0;
                    return (ushort)(_getMinVisibleHorizontalIndex() - character.GetBaseActionRange() + (characterNum - no) + 24);
                },
                _positionCalculator.GetOwnCharacterVerticalIndex
            );

        }
        /// 敵キャラクターを初期配置する。
        public void SetEnemyCharactersToInitialGrid(IEnumerable<Character> characters)
        {
            _setCharactersToInitialGrid(
                characters,
                (character, no, characterNum) =>
                {
                    if (character.IsFirstHidden) return (ushort)(HorizontalSize - 1);
                    return (ushort)(_getMaxVisibleHorizontalIndex() + character.GetBaseActionRange() - (characterNum - no) - 24);
                },
                _positionCalculator.GetEnemyCharacterVerticalIndex
            );
        }
        public void _setCharactersToInitialGrid(IEnumerable<Character> characters, Func<Character, byte, byte, ushort> getHorizontalIndex, Func<byte, byte, byte, ushort> getVerticalIndex)
        {
            var sortedCharacters = characters.OrderBy(c => (int)c.CharacterPosition).ThenBy(c => c.OrderInLayer).ToArray();
            var characterPositionToCountMap = new Dictionary<CharacterPosition, byte>();
            var characterPositionToTotalCountMap = sortedCharacters
                .ToLookup(character => character.CharacterPosition)
                .ToDictionary(group => group.Key, group => (byte)group.Count());
            var characterNum = (byte)sortedCharacters.Count();
            for (byte no = 1; no <= characterNum; no++)
            {
                var character = sortedCharacters[no - 1];
                if (!characterPositionToCountMap.ContainsKey(character.CharacterPosition))
                {
                    characterPositionToCountMap.Add(character.CharacterPosition, 0);
                }
                characterPositionToCountMap[character.CharacterPosition]++;
                var h = getHorizontalIndex(character, no, characterNum);
                var v = getVerticalIndex(characterPositionToCountMap[character.CharacterPosition], (byte)(no - characterPositionToCountMap[character.CharacterPosition]), characterPositionToTotalCountMap[character.CharacterPosition]);
                SetCharacter(h, v, character);
            }
        }
        /// 分身キャラクターを配置する。
        public void SetShadowCharacter(ShadowCharacter character)
        {
            byte? blankVerticalIndex = null;
            foreach (var v in _positionCalculator.GetShadowCharacterVerticalIndexes(character.Force))
            {
                if (!_gridContainer.HasContentByVerticalIndex(v))
                {
                    blankVerticalIndex = (byte)v;
                    break;
                }
            }
            if (blankVerticalIndex.HasValue)
            {
                SetCharacter(character.InitialHorizontalIndex, blankVerticalIndex.Value, character);
            }
        }

        /// キャラクターを歩きアニメーションで正しい位置（ワールド座標系）へ移動させる。事前に正しいグリッド座標に設置しておく必要あり。
        public IObservable<bool> SetCharactersToCorrectPositionWithWalking(IEnumerable<Character> characters, float duration)
        {
            return Observable.WhenAll(characters.Select(c => c.SetViewToCorrectPositionWithWalking(duration))).Select(_ => true);
        }
        /// キャラクターを走りアニメーションで正しい位置（ワールド座標系）へ移動させる。事前に正しいグリッド座標に設置しておく必要あり。
        public IObservable<bool> SetCharactersToCorrectPositionWithRunnging(IEnumerable<Character> characters, float duration)
        {
            return Observable.WhenAll(characters.Select(c => c.SetViewToCorrectPositionWithRunning(duration))).Select(_ => true);
        }

        /// グリッド座標から対応するワールド座標を取得する。
        public Vector2 GetPosition(Noroshi.Grid.GridPosition gridPosition)
        {
            return _getPosition(WaveNo, gridPosition);
        }
        /// グリッド座標からストーリ専用初期ウェーブのワールド座標を取得する。
        /// 仮想的な 0 ウェーブ位置を基準にやや調整。
        public Vector2 GetStoryWavePosition(Noroshi.Grid.GridPosition grid)
        {
            return _getPosition(0, grid) + Vector2.right * HORIZONTAL_LENGTH / 2;
        }
        Vector2 _getPosition(byte waveNo, Noroshi.Grid.GridPosition gridPosition)
        {
            var x = HORIZONTAL_LENGTH * 2 * (gridPosition.HorizontalIndex - (HorizontalSize - 1) / 2f) / HorizontalSize + HORIZONTAL_LENGTH * (waveNo - 1);
            var y = VERTICAL_LENGTH   * (gridPosition.VerticalIndex   - (VerticalSize   - 1) / 2f) / VerticalSize + VERTICAL_POSITION;
            // 被らないようにちょっとだけずらす
            x += HORIZONTAL_LENGTH_DIFF * (gridPosition.VerticalIndex - VerticalSize);
            return new Vector2(x, y);
        }

        /// 出現時などに利用する利用可能な開始水平方向グリッド座標を取得する。
        public ushort GetAvailableStartHorizontalIndex(Force force)
        {
            return force == Force.Own ? _getMinVisibleHorizontalIndex() : _getMaxVisibleHorizontalIndex();
        }
        /// 終端の水平方向グリッド座標を取得する。
        public ushort GetEndHorizontalIndex(Direction direction)
        {
            return (ushort)(direction == Direction.Right ? HorizontalSize - 1 : 0);
        }
        ushort _getMinVisibleHorizontalIndex()
        {
            return (ushort)((HorizontalSize - Constant.VISIBLE_FIELD_HORIZONTAL_GRID_SIZE) / 2 + 1);
        }
        ushort _getMaxVisibleHorizontalIndex()
        {
            return (ushort)(HorizontalSize - (HorizontalSize - Constant.VISIBLE_FIELD_HORIZONTAL_GRID_SIZE) / 2);
        }
        /// 前方向を取得する。
        public static Direction GetForwardDirection(Force force)
        {
            Direction direction;
            if (force == Force.Own)
            {
                direction = Direction.Right;
            }
            else if (force == Force.Enemy)
            {
                direction = Direction.Left;
            }
            else
            {
                throw new Exception();
            }
            return direction;
        }
    }
}
