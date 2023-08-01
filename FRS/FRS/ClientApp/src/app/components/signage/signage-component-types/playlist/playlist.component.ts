import { Component, Input } from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser';
import { PlaylistService } from '../../../../services/playlist.service';

import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';
import { ConfigurationService } from '../../../../services/configuration.service';

import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'signage-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class SignagePlaylist {
  @Input() configurations: any;

  @Input() preview: boolean;

  mac_address: any;
  device: any;
  locationColorTheme: any;
  locationName: any;

  private signalRCoreconnection: signalR.HubConnection;
  defaultVolume: any = 0;

  medias;

  current = 0;

  even = false;

  url;
  isVideo;
  animation;
    changeVar: any;
    data: any;
    playlist: any;

  constructor(private sanitizer: DomSanitizer, private playlistService: PlaylistService, private route: ActivatedRoute, private authService: AuthService,
    private configurationService: ConfigurationService, private deviceService: DeviceService) {

  }

  ngOnInit() {
    console.log("playlist on INIT")

    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    if (!this.preview) {
      this.route.params.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });

      if (!this.mac_address) {
        this.route.queryParams.subscribe(queryParams => {
          this.mac_address = queryParams["mac_address"];
        });
      }



      this.getDeviceInfo();
    }

    this.startPlaylist();
  }

  getData() {
    this.url = null;

    this.playlistService.getPlaylistById(this.configurations.configurationsObj.playlistId)
      .subscribe(result => {
        this.playlist = result;
        this.medias = result.images;

        if (this.medias) this.medias = this.medias.sort((a, b) => a.imageIndex - b.imageIndex);

        try {
          localStorage.setItem("playlist-" + this.configurations.configurationsObj.playlistId, JSON.stringify(this.medias));
        } catch (ex) { }
      },
        error => { });
  }

  startPlaylist() {
    console.log("start playlist", this.medias);

    try {
      if (!this.medias) {
        this.medias = JSON.parse(localStorage.getItem("playlist-" + this.configurations.configurationsObj.playlistId));
      }
    } catch (ex) { }

    let duration = 5;

    if (!this.playlist || this.playlist.id != this.configurations.configurationsObj.playlistId) this.getData();

    if (this.medias && this.medias.length > 0) {
      if (this.current >= this.medias.length) this.current = 0;

      let m = this.medias[this.current];

      this.url = m.imageLocation;
      this.isVideo = m.isVideo;

      if (m.animation) this.animation = "w3-animate-" + m.animation;
      if (m.duration) duration = m.duration;

      this.even = !this.even;

      this.current++;
    }

    setTimeout(() => {
      this.startPlaylist();
    }, duration * 1000);
  }

  getDeviceInfo() {
    if (this.mac_address) {
      this.deviceService.getDeviceById(null, this.mac_address, true)
        .subscribe(results => {

          this.device = results.data;
          if (this.device) {
            this.locationColorTheme = this.device.locationColorTheme;
            this.locationName = this.device.locationName;
            this.defaultVolume = this.device.defaultVolume || 0;

            this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/frsdevice?device_id=" + this.device.id + "&source=server", true, this.signalRCoreconnection);
            if (this.signalRCoreconnection != null) {
              this.signalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
                console.log('Refreshing Device Display');
                this.defaultVolume = deviceVM.defaultVolume || 0;
              });
            }
          }
        });
    }
  }
}
