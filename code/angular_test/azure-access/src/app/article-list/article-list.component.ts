import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

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
    this.articleService.addArticle(article);
  }

  deleteArticle(article: Article) {
    this.articleService.deleteArticle(article);
  }

  ngOnInit() {
    this.articles = this.articleService.getArticles();
  }

}
