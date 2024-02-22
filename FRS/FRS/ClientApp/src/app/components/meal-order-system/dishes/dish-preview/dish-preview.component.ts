import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Dish, DishDetail } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { ActivatedRoute } from '@angular/router';
import { FileService } from 'src/app/services/file.service';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
//import satsimage from 'src/app/assets/images/sats-logo.jpeg';

@Component({
  selector: 'dish-preview',
  templateUrl: './dish-preview.component.html',
  styleUrls: ['./dish-preview.component.css']
})
export class DishPreviewComponent implements OnInit {
  id: string;
  dish: Dish;
  totalWeight: number;
  satsLogo = 'assets\\images\\sats-logo.jpeg';
  private restrictions = [];
  restrictionTableData: any[][] = [];
  nipTableData: any[][] = [];

  constructor(private http: HttpClient, private dishService: DishService,
    private fileService: FileService, private route: ActivatedRoute, private restrictionService: RestrictionService) {
    this.route.params.subscribe(queryParams => {
      this.id = queryParams["id"];
    });
    
  }

  getRestrictions() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.restrictionService.getRestrictionsByFilter(filter)
      .subscribe(results => {
        this.restrictions = results.pagedData;
        
        let maxRowSize = this.restrictions.length > 0 ? Math.ceil(this.restrictions.length / 2) : 0;
        if (maxRowSize < 3) maxRowSize = 3;
        this.restrictions.forEach((p, index, ps) => {
          (<any>p).checked = this.dish.restrictions != null && this.dish.restrictions.findIndex(f => f.restrictionId == p.id) > -1;
        });

        let c = 0;
        for (let i = 0; i < maxRowSize; i++) {
          this.restrictionTableData[i] = [];
          let isValid = c < this.restrictions.length;
          this.restrictionTableData[i][0] = isValid? this.restrictions[c].checked : '';
          this.restrictionTableData[i][1] = isValid ? this.restrictions[c].label : '';

          isValid = c + 1 < this.restrictions.length;
          this.restrictionTableData[i][2] = isValid ? this.restrictions[c + 1].checked : '';
          this.restrictionTableData[i][3] = isValid ? this.restrictions[c + 1].label : '';
          if (isValid) c += 2;
        }
        
      },
        error => {
        });
  }

  ngOnInit() {
    if (this.id) {
      this.dishService.getDishById(this.id).subscribe(results => {
        this.dish = results;
        if (this.dish.bentoBoxTypePicture)
          this.dish.bentoBoxTypePicture = this.dish.bentoBoxTypePicture.replace(/\\/g, '/');
          this.totalWeight = this.dish.cookedWeight;
        //if (this.dish && this.dish.subDishes) {
        //  this.totalWeight += this.dish.subDishes.filter(item => item.dish.cookedWeight)
        //    .reduce((sum, current) => sum + current.dish.cookedWeight, 0);
        //}

        this.getRestrictions();

        for (let i = 0; i < 4; i++) {
          this.nipTableData[i] = [];
        }

        this.nipTableData[0][0] = 'Calories (kcal)';
        this.nipTableData[0][1] = this.dish.calories;
        this.nipTableData[0][2] = 'Carbohydrates (g)';
        this.nipTableData[0][3] = this.dish.totalCarb;
        this.nipTableData[1][0] = 'Protein (g)';
        this.nipTableData[1][1] = this.dish.protein;
        this.nipTableData[1][2] = 'Sugar (g)';
        this.nipTableData[1][3] = this.dish.sugar;
        this.nipTableData[2][0] = 'Fat (kcal)';
        this.nipTableData[2][1] = this.dish.totalFat;
        this.nipTableData[2][2] = '';
        this.nipTableData[2][3] = '';
        
      }, error => { });
    }
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }
}
