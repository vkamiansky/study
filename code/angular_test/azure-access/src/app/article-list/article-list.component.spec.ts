import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ArticleListComponent } from './article-list.component';
import { ArticleComponent } from '../article/article.component';
import { ArticleFormComponent } from '../article-form/article-form.component';
import { FormBackgroundDirective } from '../form-background.directive';
import { HoverShowDirective } from '../hover-show.directive';

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
        ]
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
});
