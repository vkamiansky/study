import { Component, OnInit } from '@angular/core';
import { Observable, BehaviorSubject, Subject, Subscriber } from 'rxjs';

import * as _ from 'underscore'
import 'msal'

import {
  Article
} from '../article';

import {
  ArticleService
} from '../article.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit {

  articles: Observable<Article[]>;

  constructor(private articleService: ArticleService) {
  }
  
  addArticle(article: Article) {
    return this.articleService.addArticle(article)
    .then(x => {
      this.articleService.updateArticles();
    });
  }

  deleteArticle(article: Article) {
    return this.articleService.deleteArticle(article)
    .then(x => {
      this.articleService.updateArticles();
    });
  }

  ngOnInit() {
    this.articles = this.articleService.articles.asObservable();
    return this.articleService.updateArticles();
  }

}
