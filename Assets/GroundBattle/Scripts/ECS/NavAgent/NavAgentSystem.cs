using UnityEngine;
using Unity.Entities;
using UnityEngine.Experimental.AI;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEditor;
using Unity.Jobs;
using JetBrains.Annotations;
using UnityEngine.AI;

[BurstCompile]
public partial struct NavAgentSystem : ISystem
{
    private EntityQuery entityQuery;
    private NavMeshWorld navMeshWorld;
    private BufferLookup<WaypointBuffer> waypointBufferLookup;

    private NativeArray<Entity> entities;
    private NativeArray<EntityCommandBuffer> ecbs;
    private NativeList<NavMeshQuery> navMeshQueries;

    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        entityQuery = new EntityQueryBuilder(Allocator.Persistent)
            .WithAll<NavAgentComponent>()
            .WithAll<LocalTransform>()
            .Build(ref state);

        navMeshWorld = NavMeshWorld.GetDefaultWorld();
        waypointBufferLookup = state.GetBufferLookup<WaypointBuffer>(true);

        navMeshQueries = new NativeList<NavMeshQuery>(Allocator.Persistent);
    }

    [BurstCompile]
    private void OnDestroy(ref SystemState state)
    {
        foreach(NavMeshQuery n in navMeshQueries)
        {
            n.Dispose();
        }

        navMeshQueries.Dispose();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        //ClassicUpdate(ref state);

        entities = entityQuery.ToEntityArray(Allocator.TempJob);
        ecbs = new NativeArray<EntityCommandBuffer>(entities.Length, Allocator.TempJob);

        for (int i = 0; i < entities.Length; i++)
        {
            ecbs[i] = new EntityCommandBuffer(Allocator.TempJob);
        }

        waypointBufferLookup.Update(ref state);

        NativeArray<JobHandle> handles = new NativeArray<JobHandle>(entities.Length, Allocator.TempJob);
        NativeArray<NavAgentComponent> agents = entityQuery.ToComponentDataArray<NavAgentComponent>(Allocator.TempJob);
        NativeArray<LocalTransform> transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        double elapsedTime = SystemAPI.Time.ElapsedTime;
        for(int i = 0; i < entities.Length; i++)
        {
            if (agents[i].querySet == false)
            {
                NavAgentComponent agent = agents[i];
                navMeshQueries.Add(new NavMeshQuery(navMeshWorld, Allocator.Persistent, 1000));
                agent.querySet = true;
                agents[i] = agent;
            }

            if (agents[i].nextPathCalculateTime < elapsedTime)
            {
                CalculatePathJob calculatePathJob = new CalculatePathJob 
                { 
                    entity = entities[i],
                    agent = agents[i],
                    fromPosition = transforms[i].Position,
                    ecb = ecbs[i],
                    query = navMeshQueries[i]
                };
                handles[i] = calculatePathJob.Schedule();
            }
            else if (agents[i].pathCalculated)
            {
                MoveJob moveJob = new MoveJob
                {
                    entity = entities[i],
                    agent = agents[i],
                    transform = transforms[i],
                    ecb = ecbs[i],
                    deltaTime = SystemAPI.Time.DeltaTime,
                    waypoints = waypointBufferLookup
                };

                handles[i] = moveJob.Schedule();
            }
        }

        JobHandle.CompleteAll(handles);

        for(int i = 0; i < entities.Length; i++)
        {
            ecbs[i].Playback(state.EntityManager);
            ecbs[i].Dispose();
        }

        agents.Dispose();
        transforms.Dispose();
        handles.Dispose();
        entities.Dispose();
        ecbs.Dispose();
    }

    [BurstCompile]
    private void ClassicUpdate(ref SystemState state)
    {
        foreach (var (navAgent, transform, entity) in SystemAPI.Query<RefRW<NavAgentComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            DynamicBuffer<WaypointBuffer> waypointBuffer = state.EntityManager.GetBuffer<WaypointBuffer>(entity);

            if (navAgent.ValueRO.nextPathCalculateTime < SystemAPI.Time.ElapsedTime)
            {
                navAgent.ValueRW.nextPathCalculateTime += 1;
                navAgent.ValueRW.pathCalculated = false;
                CalculatePath(navAgent, transform, waypointBuffer, ref state);
            }
            else
            {
                Move(navAgent, transform, waypointBuffer, ref state);
            }
        }
    }

    //CLASSIC
    [BurstCompile]
    private void Move(RefRW<NavAgentComponent> navAgent, RefRW<LocalTransform> transform, DynamicBuffer<WaypointBuffer> waypointBuffer,
        ref SystemState state)
    {
        if (math.distance(transform.ValueRO.Position, waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint) < 0.4f)
        {
            //NEXT WAYPOINT
            if (navAgent.ValueRO.currentWaypoint + 1 < waypointBuffer.Length)
            {
                navAgent.ValueRW.currentWaypoint += 1;
            }
            //ARRIVED
            else
            {
                return;
            }
        }

        float3 direciton = waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint - transform.ValueRO.Position;
        float angle = math.degrees(math.atan2(direciton.z, direciton.x));

        transform.ValueRW.Rotation = math.slerp(
                        transform.ValueRW.Rotation,
                        quaternion.Euler(new float3(0, angle, 0)),
                        SystemAPI.Time.DeltaTime);

        transform.ValueRW.Position += math.normalize(direciton) * SystemAPI.Time.DeltaTime * navAgent.ValueRO.moveSpeed;
    }

    [BurstCompile]
    private void CalculatePath(RefRW<NavAgentComponent> navAgent, RefRW<LocalTransform> transform, DynamicBuffer<WaypointBuffer> waypointBuffer,
        ref SystemState state)
    {
        NavMeshQuery query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.TempJob, 1000);

        float3 fromPosition = transform.ValueRO.Position;
        float3 toPosition = navAgent.ValueRO.targetPosition;
        float3 extents = new float3(1, 1, 1);

        NavMeshLocation fromLocation = query.MapLocation(fromPosition, extents, 0);
        NavMeshLocation toLocation = query.MapLocation(toPosition, extents, 0);

        PathQueryStatus status;
        PathQueryStatus returningStatus;
        int maxPathSize = 100;

        if (query.IsValid(fromLocation) && query.IsValid(toLocation))
        {
            status = query.BeginFindPath(fromLocation, toLocation);
            if (status == PathQueryStatus.InProgress)
            {
                status = query.UpdateFindPath(100, out int iterationsPerformed);
                if (status == PathQueryStatus.Success)
                {
                    status = query.EndFindPath(out int pathSize);

                    NativeArray<NavMeshLocation> result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                    NativeArray<StraightPathFlags> straightPathFlag = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                    NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                    NativeArray<PolygonId> polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                    int straightPathCount = 0;

                    query.GetPathResult(polygonIds);

                    returningStatus = PathUtils.FindStraightPath
                        (
                        query,
                        fromPosition,
                        toPosition,
                        polygonIds,
                        pathSize,
                        ref result,
                        ref straightPathFlag,
                        ref vertexSide,
                        ref straightPathCount,
                        maxPathSize
                        );

                    if (returningStatus == PathQueryStatus.Success)
                    {
                        waypointBuffer.Clear();

                        foreach (NavMeshLocation location in result)
                        {
                            if (location.position != Vector3.zero)
                            {
                                waypointBuffer.Add(new WaypointBuffer { wayPoint = location.position });
                            }
                        }

                        navAgent.ValueRW.currentWaypoint = 0;
                        navAgent.ValueRW.pathCalculated = true;
                    }
                    straightPathFlag.Dispose();
                    polygonIds.Dispose();
                    vertexSide.Dispose();
                }
            }
        }
        query.Dispose();
    }

    //JOBS
    [BurstCompile]
    private struct MoveJob : IJob
    {
        public NavAgentComponent agent;
        public LocalTransform transform;
        public Entity entity;
        public float deltaTime;
        public EntityCommandBuffer ecb;
        [ReadOnly] public BufferLookup<WaypointBuffer> waypoints;

        public void Execute()
        {
            if (waypoints.TryGetBuffer(entity, out DynamicBuffer<WaypointBuffer> waypointBuffer) && math.distance(transform.Position, waypointBuffer[agent.currentWaypoint].wayPoint) < 0.4f)
            {
                //NEXT WAYPOINT
                if (agent.currentWaypoint + 1 < waypointBuffer.Length)
                {
                    agent.currentWaypoint += 1;
                    ecb.SetComponent(entity, agent);
                }
                //ARRIVED
                else
                {
                    return;
                }
            }

            float3 direciton = waypointBuffer[agent.currentWaypoint].wayPoint - transform.Position;
            float angle = math.degrees(math.atan2(direciton.z, direciton.x));

            transform.Rotation = math.slerp(
                            transform.Rotation,
                            quaternion.Euler(new float3(0, angle, 0)),
                            deltaTime);

            transform.Position += math.normalize(direciton) * deltaTime * agent.moveSpeed;
            ecb.SetComponent(entity, transform);
        }
    }
    [BurstCompile]
    private struct CalculatePathJob : IJob
    {
        public Entity entity;
        public NavAgentComponent agent;
        public EntityCommandBuffer ecb;
        public float3 fromPosition;
        public NavMeshQuery query;

        public void Execute()
        {
            agent.nextPathCalculateTime += 1;
            agent.pathCalculated = false;
            ecb.SetComponent(entity, agent);

            float3 toPosition = agent.targetPosition;
            float3 extents = new float3(1, 1, 1);

            NavMeshLocation fromLocation = query.MapLocation(fromPosition, extents, 0);
            NavMeshLocation toLocation = query.MapLocation(toPosition, extents, 0);

            PathQueryStatus status;
            PathQueryStatus returningStatus;
            int maxPathSize = 100;

            if (query.IsValid(fromLocation) && query.IsValid(toLocation))
            {
                status = query.BeginFindPath(fromLocation, toLocation);
                if (status == PathQueryStatus.InProgress || status == PathQueryStatus.Success)
                {
                    status = query.UpdateFindPath(100, out int iterationsPerformed);
                    if (status == PathQueryStatus.Success)
                    {
                        status = query.EndFindPath(out int pathSize);

                        NativeArray<NavMeshLocation> result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                        NativeArray<StraightPathFlags> straightPathFlag = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                        NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                        NativeArray<PolygonId> polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                        int straightPathCount = 0;

                        query.GetPathResult(polygonIds);

                        returningStatus = PathUtils.FindStraightPath
                            (
                            query,
                            fromPosition,
                            toPosition,
                            polygonIds,
                            pathSize,
                            ref result,
                            ref straightPathFlag,
                            ref vertexSide,
                            ref straightPathCount,
                            maxPathSize
                            );

                        if (returningStatus == PathQueryStatus.Success)
                        {
                            ecb.SetBuffer<WaypointBuffer>(entity);

                            foreach (NavMeshLocation location in result)
                            {
                                if (location.position != Vector3.zero)
                                {
                                    ecb.AppendToBuffer(entity, new WaypointBuffer { wayPoint = location.position });
                                }
                            }

                            agent.currentWaypoint = 0;
                            agent.pathCalculated = true;
                            ecb.SetComponent(entity, agent);
                        }
                        straightPathFlag.Dispose();
                        polygonIds.Dispose();
                        vertexSide.Dispose();
                    }
                }
            }
        }
    }
}