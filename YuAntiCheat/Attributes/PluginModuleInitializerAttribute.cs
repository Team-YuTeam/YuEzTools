namespace YuAntiCheat.Attributes;

/// <summary>
///在 <see cref="Main.Load"/> 中用于启动时初始化的方法<br/>
///在 static 方法前加上 [PluginModuleInitializer] ，启动时自动调用<br/>
/// 使用 [PluginModuleInitializer(InitializePriority.High)] 可以指定调用顺序
/// </summary>
public sealed class PluginModuleInitializerAttribute : InitializerAttribute<PluginModuleInitializerAttribute> { }