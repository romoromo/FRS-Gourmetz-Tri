import { Image } from '../models/image.model';

export class Playlist {

  constructor(id?: string, name?: string, description?: string) {

    this.id = id;
    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public imageIds: imageIndex[];
  public images: Image[];
  public imageTitle: string;
  public institutionId: string;
}

export class imageIndex {

  constructor(imageId?: number, imgIndex?: number, duration?: number, animation?: string) {

    this.imageId = imageId;
    this.imgIndex = imgIndex;
    this.duration = duration;
    this.animation = animation;
  }

  public imageId: number;
  public imgIndex: number;
  public duration: number;
  public animation: string;
}
