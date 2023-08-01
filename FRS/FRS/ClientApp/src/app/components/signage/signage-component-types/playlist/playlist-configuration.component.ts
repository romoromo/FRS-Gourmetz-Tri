import { Component, Input } from '@angular/core';
import { PlaylistService } from '../../../../services/playlist.service';
import { Playlist } from '../../../../models/playlist.model';

@Component({
  selector: 'playlist-configuration',
  templateUrl: './playlist-configuration.component.html'

})
export class SignagePlaylistConfiguration {
  @Input() configurations: any = {};
    allPlaylist: Playlist[];

  constructor(public playlistService: PlaylistService) { }

  getConfigurations() {
    return this.configurations;
  }

  ngOnInit() {
    this.playlistService.getPlaylists(-1, -1, null)
      .subscribe(results => {


        this.allPlaylist = results;


      },
        error => { });
  }
}
