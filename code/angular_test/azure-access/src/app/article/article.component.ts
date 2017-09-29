import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
} from '@angular/core';

import {
  Article
} from '../article';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.css']
})
export class ArticleComponent implements OnInit {

  constructor() { }

  @Input('article') data: Article;
  @Output() articleDeleted = new EventEmitter<Article>();
  deleteArticle() {
    this.articleDeleted.emit(this.data);
    }

  ngOnInit() {
  }

}
