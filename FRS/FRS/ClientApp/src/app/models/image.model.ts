
export class Image {

  constructor(id?: string, title?: string, imageLocation?: string, duration?: number, animation?: string) {

    this.id = id;
    this.title = title;
    this.imageLocation = imageLocation;
    this.duration = duration,
      this.animation = animation
  }

  public id: string;
  public title: string;
  public imageLocation: string;
  public institutionId: string;

  public isVideo: boolean;

  public mediaId: string;

  public description: string;

  public tags: string;

  public duration?: number;
  public animation?: string;
}
