using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Arch.Unity.Toolkit
{
    // TODO: optimize register/unregister

    public sealed class SystemRunner : ISystemRunner
    {
        static SystemRunner()
        {
            Initialization = new SystemRunner(PlayerLoopTiming.Initialization);
            EarlyUpdate = new SystemRunner(PlayerLoopTiming.EarlyUpdate);
            FixedUpdate = new SystemRunner(PlayerLoopTiming.FixedUpdate);
            PreUpdate = new SystemRunner(PlayerLoopTiming.PreUpdate);
            Update = new SystemRunner(PlayerLoopTiming.Update);
            PreLateUpdate = new SystemRunner(PlayerLoopTiming.PreLateUpdate);
            PostLateUpdate = new SystemRunner(PlayerLoopTiming.PostLateUpdate);
            TimeUpdate = new SystemRunner(PlayerLoopTiming.TimeUpdate);

            Default = Update;
        }

        SystemRunner(PlayerLoopTiming playerLoopTiming)
        {
            this.playerLoopTiming = playerLoopTiming;
        }

        readonly PlayerLoopTiming playerLoopTiming;
        readonly List<UnitySystemBase> systems = new();

        public static ISystemRunner Default { get; set; }

        public static readonly ISystemRunner Initialization;
        public static readonly ISystemRunner EarlyUpdate;
        public static readonly ISystemRunner FixedUpdate;
        public static readonly ISystemRunner PreUpdate;
        public static readonly ISystemRunner Update;
        public static readonly ISystemRunner PreLateUpdate;
        public static readonly ISystemRunner PostLateUpdate;
        public static readonly ISystemRunner TimeUpdate;

        public void Add(UnitySystemBase system)
        {
            systems.Add(system);
        }

        public void Remove(UnitySystemBase system)
        {
            systems.Remove(system);
        }

        static Dictionary<Type, string> names = new();

        public void Run()
        {
            var state = new SystemState()
            {
                Time = GetElaspedTime(playerLoopTiming),
                DeltaTime = GetDeltaTime(playerLoopTiming),
            };

            foreach (var system in systems)
            {
                if (!names.TryGetValue(system.GetType(), out var name))
                {
                    name = names[system.GetType()] = system.GetType().FullName;
                }

                Profiler.BeginSample(name);
                
                try { system.BeforeUpdate(state); }
                catch (Exception ex) { Debug.LogException(ex); }
                try { system.Update(state); }
                catch (Exception ex) { Debug.LogException(ex); }
                try { system.AfterUpdate(state); }
                catch (Exception ex) { Debug.LogException(ex); }

                Profiler.EndSample();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDeltaTime(PlayerLoopTiming playerLoopTiming)
        {
            return playerLoopTiming == PlayerLoopTiming.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double GetElaspedTime(PlayerLoopTiming playerLoopTiming)
        {
            return playerLoopTiming == PlayerLoopTiming.FixedUpdate ? Time.fixedTimeAsDouble : Time.timeAsDouble;
        }
    }
}