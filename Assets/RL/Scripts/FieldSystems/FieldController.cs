﻿using System.Collections.Generic;
using System.Linq;
using HK.Framework.EventSystems;
using RL.ActorControllers;
using RL.Events.FieldSystems;
using RL.Extensions;
using RL.FieldSystems.Generators.Field;
using RL.GameSystems;
using UnityEngine;
using UnityEngine.Assertions;

namespace RL.FieldSystems
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldController : MonoBehaviour
    {
        [SerializeField]
        private CellController cellPrefab;

        [SerializeField]
        private int matrixNumber;

        private CellController[,] cells;

        private readonly List<List<Room>> roomMatrix = new List<List<Room>>();

        private readonly List<Room> allRoom = new List<Room>();

        private float size;

        public void CreateCells()
        {
            if (this.matrixNumber <= 0)
            {
                Assert.IsTrue(false, "matrixNumberが0なためセルを生成できません");
                return;
            }

            this.cells = new CellController[this.matrixNumber, this.matrixNumber];
            var t = (RectTransform)this.transform;
            Assert.IsNotNull(t);
            this.size = (t.rect.width / this.matrixNumber);

            for (int y = 0; y < this.matrixNumber; y++)
            {
                for (int x = 0; x < this.matrixNumber; x++)
                {
                    var cell = Instantiate(this.cellPrefab, t);
                    var id = new Point(x, y);
                    cell.Initialize(id, GetPosition(id), this.size);
                    this.cells[y, x] = cell;
                }
            }
        }

        public void GenerateField(IFieldGenerator generator)
        {
            for (int y = 0; y < this.matrixNumber; y++)
            {
                for (int x = 0; x < this.matrixNumber; x++)
                {
                    this.cells[y, x].Clear();
                }
            }

            Actor.ClearEnemies();
            ClearRoom();
            generator.Generate();
            Broker.Global.Publish(GeneratedField.Get());
        }

        public static void ClearRoom()
        {
            RoomMatrix.Clear();
            AllRoom.Clear();
        }

        public static Vector2 GetPosition(Point id)
        {
            var instance = GameController.Instance.FieldController;
            return new Vector2(id.x * instance.size, -id.y * instance.size);
        }

        public static bool CanMove(Point current, Point next)
        {
            // フィールド外へ侵入しようとしてたら移動できない
            if (next.x < 0 || next.x >= XMax || next.y < 0 || next.y >= YMax)
            {
                return false;
            }

            var diff = next - current;
            var amount = diff.Abs;
            for (var y = 0; y <= amount.y; y++)
            {
                for (var x = 0; x <= amount.x; x++)
                {
                    var targetId = current + (diff.Vertical * y) + (diff.Horizontal * x);
                    if (!Cells[targetId.y, targetId.x].CanMove)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static float Size
        {
            get
            {
                return GameController.Instance.FieldController.size;
            }
        }

        public static CellController[,] Cells
        {
            get
            {
                return GameController.Instance.FieldController.cells;
            }
        }

        public static List<List<Room>> RoomMatrix
        {
            get
            {
                return GameController.Instance.FieldController.roomMatrix;
            }
        }

        public static List<Room> AllRoom
        {
            get
            {
                return GameController.Instance.FieldController.allRoom;
            }
        }

        public static CellController RandomCell
        {
            get
            {
                var room = AllRoom[Random.Range(0, AllRoom.Count)];
                return room.Cells[Random.Range(0, room.Cells.Length)];
            }
        }

        /// <summary>
        /// <see cref="Actor"/>が乗っていないセルを返す
        /// </summary>
        public static CellController RandomCellNotRideActor
        {
            get
            {
                // Actorで埋まっていない部屋を取得する
                var possibleRooms = AllRoom.Where(r => r.ExistActorNumber != r.Cells.Length);
                Assert.AreNotEqual(possibleRooms.Count(), 0, $"{typeof(Actor).Name}が存在しない{typeof(Room).Name}の取得に失敗しました　");
                
                var room = possibleRooms.ElementAt(Random.Range(0, possibleRooms.Count()));
                var cells = room.Cells.Where(c => c.RideActor == null);
                return cells.ElementAt(Random.Range(0, cells.Count()));
            }
        }

        public static Point RandomPosition
        {
            get
            {
                return RandomCell.Id;
            }
        }

        public static int XMax
        {
            get
            {
                return GameController.Instance.FieldController.matrixNumber;
            }
        }

        public static int YMax
        {
            get
            {
                return GameController.Instance.FieldController.matrixNumber;
            }
        }
    }
}
