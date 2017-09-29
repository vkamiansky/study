import {
  Component,
  OnInit,
  Output,
  EventEmitter
} from '@angular/core';

import {
  Article
} from '../article';

@Component({
  selector: 'app-article-form',
  templateUrl: './article-form.component.html',
  styleUrls: ['./article-form.component.css']
})
export class ArticleFormComponent implements OnInit {

  @Output() articleCreated = new EventEmitter<Article>();
  createArticle(heading: string, summary: string, text: string) {
    this.articleCreated.emit({id: -1, showDetailed: true, heading: heading, summary: summary, text: text});
    }

  constructor() { }

  ngOnInit() {
  }

}
