
export class Department {

  constructor(id?: string, name?: string, description?: string) {

    this.id = id;
    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public institutionId: string;
}

export class PIBTemplate {

  constructor(template_body?: string, name?: string, imgUri?: string, device_api_url?: string, is_post_to_device?: boolean, id?: string, description?: string) {

    this.templateBody = template_body;
    this.name = name;
    this.imgUrl = imgUri;
    this.deviceAPIUrl = device_api_url;
    this.isPostToDevice = is_post_to_device;
    this.id = id;
    this.description = description;
  }

  public id: string;
  public templateBody: string;
  public name: string;
  public description: string;
  public imgUrl: string;
  public deviceAPIUrl: string;
  public isPostToDevice: boolean;
  public isMapToAPI: boolean;
  public isStaticLink: boolean;
  public macAddress: string;
  public epaperUrl: string;
  public locationIds: any = [];
  public locations: PIBTemplateLocation[] = [];
}

export class PIBTemplateLocation {

  constructor() {
  }

  //public id: string;
  public location_id: string;
  public hospital_id: string;
  public code: string;
  public label: string;
  public alias: string;
  public sequence: number;
  public ward: boolean;
  public bed: boolean;
  public pibTemplateId: string;
}

export class PIBDevice {

  constructor() {
  }

  public id: string;
  public device_code: string;
  public device_status: string;
  public device_label: string;
  public mac_address: string;
  public ip_address: string;
  public location_id: string;
  public host_address: string;
  public last_heartbeat?: Date;
  public location_code: string;
  public epaper_url: string;
}
