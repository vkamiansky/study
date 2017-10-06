import { Component, OnInit } from '@angular/core';

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
  providers: [ArticleService],
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit {

  articles: Article[];
  constructor(private articleService: ArticleService) {
  }
  
  addArticle(article: Article) {
    if(-1 == article.id)
    { 
      article.id = 1 + this.articles.reduce((a, x) => x.id > a ? x.id: a, -1);
    }
    this.articles.unshift(article);
  }

  deleteArticle(article: Article) {
    let index = this.articles.findIndex((o) => o.id == article.id);
    if(-1 != index)
    {
      this.articles.splice(index, 1);
    }
  }

  ngOnInit() {
    this.articleService.getArticles().then(articles => this.articles = articles);
  }

}
