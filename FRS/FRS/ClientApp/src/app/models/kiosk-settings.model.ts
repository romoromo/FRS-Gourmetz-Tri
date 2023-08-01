import { Image } from '../models/image.model';
import { Playlist } from '../models/playlist.model';

export class KioskSettings {
  public floor_interval: number;
  public arrow_image: string;
    arrow_color: string;
    arrow_speed: number;
    line_color: string;

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
  }

  public id: string;
  public label: string;
  public screenSaveTime: number;
  public screen_save_time: number;
  public ssPlaylistId: string;
  public ss_playlist_id: string;
  public ssPlaylist: Playlist;
  public bannerId: string;
  public banner_id: string;
  public bannerImage: Image;
  public topBannerId: string;
  public top_banner_id: string;
  public topBanner: Playlist;
  public bottomBannerId: string;
  public bottom_banner_id: string;
  public bottomBanner: Playlist;
  public routeWeight: number;
  public route_weight: number;
  public routeSpeed: number;
  public route_speed: number;
  public zoom_level: number;
  public routeColor: string;
  public route_color: string;
  public jan: string;
  public feb: string;
  public mar: string;
  public apr: string;
  public may: string;
  public jun: string;
  public jul: string;
  public aug: string;
  public sep: string;
  public okt: string;
  public nov: string;
  public des: string;

  public dirColor: string;
  public dir_color: string;
  public dirFontColor: string;
  public dir_font_color: string;
  public dirFontType: string;
  public dir_font_type: string;

  public dir_font_type2: string;

  public dir_font_size: number;

  public baloon_image: string;

  public baloon_width: number;

  public baloon_height: number;

  public anchor_h: number;

  public anchor_v: number;

  public defEventId: number;
  public def_event_id: number;
  public defEvent: Playlist;
}
