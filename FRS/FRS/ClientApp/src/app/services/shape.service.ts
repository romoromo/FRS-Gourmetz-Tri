import { Injectable } from '@angular/core';
import Konva from 'konva';
@Injectable({
  providedIn: 'root'
})
export class ShapeService {
  constructor() { }
  circle() {
    return new Konva.Circle({
      x: 100,
      y: 100,
      radius: 70,
      fill: 'red',
      stroke: 'black',
      strokeWidth: 4,
      draggable: true
    });
  }
  line(pos, mode: string = 'brush') {
    return new Konva.Line({
      stroke: 'black',
      strokeWidth: mode === 'brush' ? 2 : 30,
      globalCompositeOperation:
        mode === 'brush' ? 'source-over' : 'destination-out',
      points: [pos.x, pos.y],
      draggable: mode == 'brush'
    });
  }
  rectangle() {
    return new Konva.Rect({
      x: 20,
      y: 20,
      width: 100,
      height: 50,
      fill: 'black',
      stroke: 'black',
      strokeWidth: 2,
      draggable: true
    });
  }
  image(image: CanvasImageSource, attrs: any) {
    var x = attrs.x === undefined ? 0 : parseInt(attrs.x);
    var y = attrs.y === undefined ? 0 : parseInt(attrs.y);
    var img = new Konva.Image({
      x: x,
      y: y,
      //width: 200,
      //height: 137,
      //stroke: 'red',
      //strokeWidth: 10,
      draggable: true,
      image: image
    });
    img.setAttrs(attrs);
    img.x(x);
    img.y(y);
    return img;
  }

}
