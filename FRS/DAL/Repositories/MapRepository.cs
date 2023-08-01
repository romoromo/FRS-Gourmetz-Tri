using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using System.Diagnostics;

namespace DAL.Repositories
{
    public class MapRepository : Repository<Map>, IMapRepository
    {
        public MapRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Map> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Map> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public Map GetMapByFloorId(int id)
        {
            Map query = _appContext.Map
                .FirstOrDefault(e => e.IsActive && e.FloorId == id);

            return query;
        }

        public Point GetPointById(int idp)
        {

            //Point query =  _appContext.Point
            //    .FirstOrDefault(e => e.Id == id);

            var points = _appContext.PointDirectoryListing.Where(d => d.IsActive && d.DirectoryListingId == idp && d.Point != null && d.Point.IsActive).ToList();
            var point = points.FirstOrDefault()?.Point;

            return point;
        }

        Point GetMin(Dictionary<int, Point> unvisited, Dictionary<int, Distance> distance)
        {
            int min = int.MaxValue;
            Point result = null;
            foreach (KeyValuePair<int, Point> entry in unvisited)
            {
                var dist = distance[entry.Key];
                if (dist.Dist < min)
                {
                    min = dist.Dist;
                    result = entry.Value;
                }
            }

            return result;
        }

        public BaseOperationResponse Dijkstra(Point startPoint, Point destPoint, bool wheelchair, bool sheltered, BaseOperationResponse result)
        {
            var unvisited = new Dictionary<int, Point>();
            var distance = new Dictionary<int, Distance>();

            unvisited.Add(startPoint.Id, startPoint);
            distance.Add(startPoint.Id, new Distance
            {
                Dist = 0,
                Prev = null
            });

            //Rec(visited, unvisited, distance, destPoint);

            while(unvisited.Count() > 0)
            {
                var min = GetMin(unvisited, distance);
                unvisited.Remove(min.Id);

                if (min.Id == destPoint.Id) 
                    break;
                else
                {
                    var lines = _appContext.Line.Where(e => e.IsActive && (e.Point0Id == min.Id || e.Point1Id == min.Id) && e.Point0 != null && e.Point0.IsActive && e.Point1 != null && e.Point1.IsActive);

                    lines.ToList().ForEach(l =>
                    {
                        var next = (min.Id == l.Point0Id) ? l.Point1 : l.Point0;

                        if (wheelchair && next.NoWheelchair) return;

                        if (sheltered && next.NoSheltered) return;

                        if (next.MapId == null) return;

                        if (!distance.ContainsKey(next.Id))
                        {
                            distance.Add(next.Id, new Distance
                            {
                                Dist = int.MaxValue,
                                Prev = min
                            });

                            unvisited.Add(next.Id, next);
                        }

                        if (unvisited.ContainsKey(next.Id)) {
                            var dist = Convert.ToInt32(l.isFloorConnector ? 1 : Math.Pow(l.Point0.x - l.Point1.x, 2) + Math.Pow(l.Point0.y - l.Point1.y, 2));

                            if (distance[min.Id].Dist + dist < distance[next.Id].Dist)
                            {
                                distance[next.Id].Dist = distance[min.Id].Dist + dist;
                                distance[next.Id].Prev = min;
                            }
                        }
                    });
                }
            }

            if(!distance.ContainsKey(destPoint.Id))
            {
                result.Message = "No path can be found between two directories.";
                result.IsSuccess = false;
                return result;
            }

            var data = new List<Point>();

            var point = destPoint; 
            while(point != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(point.Label) && point.Directorys.Count() > 0)
                    {
                        point.Label = point.Directorys.ToList()[0].DirectoryListing.Label;
                    }
                } catch(Exception ex) { }

                point.Directorys = null;
                data.Add(point);
                point = distance[point.Id].Prev;
            }

            data.Reverse();

            result.Data = data;
            result.IsSuccess = true;

            return result;
        }


        public async Task<BaseOperationResponse> GetRoute(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered)
        {
            var result = new BaseOperationResponse();

            var startPts = _appContext.PointDirectoryListing.Where(d => d.IsActive && d.DirectoryListingId == startDirectoryId && d.Point != null && d.Point.IsActive && d.Point.MapId != null && d.Point.Map != null && d.Point.Map.IsActive).ToList();
            var endPts = _appContext.PointDirectoryListing.Where(d => d.IsActive && d.DirectoryListingId == destDirectoryId && d.Point != null && d.Point.IsActive && d.Point.MapId != null && d.Point.Map != null && d.Point.Map.IsActive).ToList();

            var startPoint = startPts.FirstOrDefault()?.Point;
            var destPoint = endPts.FirstOrDefault()?.Point;

            if(startPoint == null || destPoint == null)
            {
                result.Message = "Directory does not assign to any coordinate on map.";
                result.IsSuccess = false;
                return result;
            }

            return Dijkstra(startPoint, destPoint, wheelchair, sheltered, result);
        }

        public async Task<BaseOperationResponse> GetRouteTest(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered)
        {
            var result = new BaseOperationResponse();


            var startPts = _appContext.PointDirectoryListing.Where(d => d.IsActive && d.DirectoryListingId == startDirectoryId && d.Point != null && d.Point.IsActive && d.Point.MapId != null && d.Point.Map != null && d.Point.Map.IsActive).ToList();
            var endPts = _appContext.PointDirectoryListing.Where(d => d.IsActive && d.DirectoryListingId == destDirectoryId && d.Point != null && d.Point.IsActive && d.Point.MapId != null && d.Point.Map != null && d.Point.Map.IsActive).ToList();

            var startPoint = startPts.FirstOrDefault()?.Point;
            var destPoint = endPts.FirstOrDefault()?.Point;

            if (startPoint == null || destPoint == null)
            {
                result.Message = "Directory does not assign to any coordinate on map.";
                result.IsSuccess = false;
                return result;
            }

            var pointMap = new Dictionary<int, Point>();
            var unvisited = new List<Point>();

            pointMap.Add(startPoint.Id, startPoint);
            unvisited.Add(startPoint);

            while (unvisited.Count() > 0)
            {
                var p = unvisited[0]; 
                unvisited.RemoveAt(0);

                var lines = _appContext.Line.Where(e => e.IsActive && (e.Point0Id == p.Id || e.Point1Id == p.Id) && e.Point0 != null && e.Point0.IsActive && e.Point1 != null && e.Point1.IsActive);

                lines.ToList().ForEach(l =>
                {
                    var next = (p.Id == l.Point0Id) ? l.Point1 : l.Point0;

                    if (wheelchair && next.NoWheelchair) return;

                    if (sheltered && next.NoSheltered) return;

                    if (!pointMap.ContainsKey(next.Id))
                    {
                        pointMap.Add(next.Id, next);

                        unvisited.Add(next);
                    }
                });
            }

            result.Data = pointMap.Values.ToList();

            return result;
        }

        public async Task<List<Map>> GetMapsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<Map> query = _appContext.Map
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<Line>> GetConnectors(int pointId)
        {
            IQueryable<Line> query = _appContext.Line
                .Where(e => e.IsActive && e.isFloorConnector && (e.Point0Id == pointId || e.Point1Id == pointId) && e.Point0 != null && e.Point0.IsActive && e.Point1 != null && e.Point1.IsActive);

            var lines = await query.ToListAsync();

            return lines;
        }

        public async Task<List<Point>> GetFloorConnectors(int? mapId = null)
        {
            IQueryable<Point> query = _appContext.Point
                .Where(e => e.IsActive && e.IsFloorConnector && e.MapId != mapId);

            var lines = await query.ToListAsync();

            return lines;
        }

        void SavePointsLines (Map source, Map dest)
        {
            source.Points?.ToList().ForEach(cSource =>
            {
                if (cSource.IsActive || cSource.Id > 0)
                {
                    var ct = _appContext.Point.FirstOrDefault(c => c.Id == cSource.Id || c.Code == cSource.Code) ?? new Point();
                    ct.CopyFrom(cSource);

                    

                    if(ct.MapId != dest.Id) ct.MapId = dest.Id;
                    ct.IsActive = cSource.IsActive;

                    if(ct.Id == 0) _appContext.Point.Update(ct);
                    //_appContext.Entry(ct).Reference(x => x.Map).IsModified = false;
                    //_appContext.Entry(ct).Collection(x => x.Directorys).IsModified = false;
                    _appContext.SaveChanges();

                    cSource.Directorys?.ToList().ForEach(dS =>
                    {
                        if (dS.IsActive || dS.Id > 0)
                        {
                            var dt = (dS.Id > 0 ? _appContext.PointDirectoryListing.FirstOrDefault(c => c.Id == dS.Id) : _appContext.PointDirectoryListing.FirstOrDefault(c => c.DirectoryListingId == dS.DirectoryListingId && c.PointId == ct.Id)) ?? new PointDirectoryListing();
                            dt.CopyFrom(dS);

                           

                            if(dS.DirectoryListingId != null && dt.DirectoryListingId != dS.DirectoryListingId) dt.DirectoryListingId = dS.DirectoryListingId;
                            if (dt.PointId != ct.Id) dt.PointId = ct.Id;
                            dt.IsActive = dS.IsActive;



                            if (dt.Id == 0) _appContext.PointDirectoryListing.Update(dt);
                            //_appContext.Entry(dt).Reference(x => x.Point).IsModified = false;
                            _appContext.SaveChanges();
                        }
                    });
                }
            });

            

            source.Lines?.ToList().ForEach(cSource =>
            {
                if (cSource.IsActive || cSource.Id > 0)
                {
                    var ct = _appContext.Line.FirstOrDefault(c => c.Id == cSource.Id || c.Code == cSource.Code) ?? new Line();
                    ct.CopyFrom(cSource);

                    if(ct.MapId != dest.Id && !cSource.isFloorConnector) ct.MapId = dest.Id;
                    if (cSource.Point0Id != null && ct.Point0Id != cSource.Point0Id) ct.Point0Id = cSource.Point0Id;
                    if (cSource.Point1Id != null && ct.Point1Id != cSource.Point1Id) ct.Point1Id = cSource.Point1Id;
                    if (ct.Point0Id == null) ct.Point0Id = _appContext.Point.FirstOrDefault(c => c.Code == cSource.Code0 && c.IsActive)?.Id;
                    if (ct.Point1Id == null) ct.Point1Id = _appContext.Point.FirstOrDefault(c => c.Code == cSource.Code1 && c.IsActive)?.Id;
                    ct.IsActive = cSource.IsActive;


                    if(ct.Id == 0) _appContext.Line.Update(ct);
                }
            });

            _appContext.SaveChanges();
        } 

        public async Task<BaseOperationResponse> CreateAsync(Map map)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == map.Code).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = new Map();
            f.CopyFrom(map);

            f.FloorId = map.FloorId;

            await AddAsync(f);
            //_appContext.Entry(f).Collection(x => x.Lines).IsModified = false;
            //_appContext.Entry(f).Collection(x => x.Points).IsModified = false;
            //_appContext.Entry(f).Reference(x => x.Floor).IsModified = false;
            if (await _appContext.SaveChangesAsync() > 0)
            {
                SavePointsLines(map, f);

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Map map)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == map.Code && e.Id != map.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == map.Id);

            f.CopyFrom(map);
            //f.Points = null;
            //f.Lines = null;
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                SavePointsLines(map, f);

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int mapId)
        {
            var result = new BaseOperationResponse();
            var map = await GetSingleOrDefaultAsync(r => r.Id == mapId);

            if (map != null)
                return await Delete(map);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Map map)
        {
            var result = new BaseOperationResponse();
            SoftDelete(map);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }

    class Distance {
        public int Dist { get; set; }
        public Point Prev { get; set; }
    }
}
