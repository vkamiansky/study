import { async, inject, fakeAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ArticleListComponent } from './article-list.component';
import { ArticleComponent } from '../article/article.component';
import { ArticleFormComponent } from '../article-form/article-form.component';
import { FormBackgroundDirective } from '../form-background.directive';
import { HoverShowDirective } from '../hover-show.directive';
import { Article } from '../article';
import { ArticleService } from '../article.service';

@Injectable()
export class MockArticleService {

  constructor() { }

  getArticles() : Observable<Article[]> {
    return Observable.fromPromise(Promise.resolve([
      {
        showDetailed: true,
        id: 1,
        heading: "A1",
        summary: "B1",
        text: "C1"
      },
      {
        showDetailed: true,
        id: 2,
        heading: "A2",
        summary: "B2",
        text: "C2"
      }
      ]));
  }
}

describe('ArticleListComponent', () => {
  let component: ArticleListComponent;
  let fixture: ComponentFixture<ArticleListComponent>;
  let mockService = new MockArticleService();

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
          {provide: ArticleService, useValue: mockService},
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

  it('should be right', () => {
    component.articles.subscribe(x => expect(x[0].heading).toEqual('A1'));
  });
});
