import { async, inject, fakeAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';

import { ArticleListComponent } from './article-list.component';
import { ArticleComponent } from '../article/article.component';
import { ArticleFormComponent } from '../article-form/article-form.component';
import { FormBackgroundDirective } from '../form-background.directive';
import { HoverShowDirective } from '../hover-show.directive';
import { Article } from '../article';
import { ArticleService } from '../article.service';

@Injectable()
export class MockArticleService {

  private _articles: Article[];  
  articles: BehaviorSubject<Article[]>;

  constructor() {
    this._articles = [
      {
        showDetailed: true,
        id: 1,
        heading: "Initialized",
        summary: "B1",
        text: "C1"
      }
      ];
    this.articles = new BehaviorSubject<Article[]>([]);
  }

  addArticle(article: Article) : Promise<any> {
    return new Promise((resolve, reject) => {
      this._articles = [
        {
          showDetailed: true,
          id: 1,
          heading: "Added",
          summary: "B1",
          text: "C1"
        }
        ];
      resolve();
    });
  }

  deleteArticle(article: Article) : Promise<any> {
    return new Promise((resolve, reject) => {
      this._articles = [
        {
          showDetailed: true,
          id: 1,
          heading: "Deleted",
          summary: "B1",
          text: "C1"
        }
        ];
      resolve();
    });
  }

  updateArticles() : Promise<any> {
    return new Promise((resolve, reject) => {
      this.articles.next(this._articles);
      resolve();
    });
  }
}

describe('ArticleListComponent', () => {
  let component: ArticleListComponent;
  let fixture: ComponentFixture<ArticleListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        ArticleListComponent,
        ArticleComponent,
        ArticleFormComponent,
        FormBackgroundDirective,
        HoverShowDirective
        ],
      providers: [
          {provide: ArticleService, useClass: MockArticleService},
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ArticleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });

  it('should be initialized correctly', (done) => {
    component.articles.subscribe(x => {
      expect(x[0].heading).toEqual('Initialized');
      done();
    });
  });

  it('should work right on addition', (done) => {
    component.addArticle(null).then(() =>
    component.articles.subscribe(x => {
      expect(x[0].heading).toEqual('Added');
      done();
    }));
  });

  it('should work right on deletion', (done) => {
    component.deleteArticle(null).then(() =>
    component.articles.subscribe(x => {
      expect(x[0].heading).toEqual('Deleted');
      done();
    }));
  });
});
