namespace YuEzTools.EventDetector;

public class EventType
{
    public struct GameEvent
    {
        public string EventTime { get; }
        public EventTypes EventType { get; }
        public string Thing { get; }
        public PlayerControl Player { get; }
        public PlayerControl Target { get; }

        public GameEvent(string eventTime, EventTypes eventType,string thing,PlayerControl player,PlayerControl target)
        {
            EventTime = eventTime;
            EventType = eventType;
            Thing = thing;
            Player = player;
            Target = target;
        }
    }

    public enum EventTypes
    {
        MurderPlayer,  // 击杀
        Sabotage,  // 破坏
        VotePlayer,  // 投票
        Meeting,  // 紧急会议和报告尸体
        DoTask,  // 做任务
        FixSabotage,  // 修复破坏
        RoleChange,  // 角色更改
        ShapeShift,  // 变形
        Vanish,  // 隐身
        Track,  // 追踪
    }
}