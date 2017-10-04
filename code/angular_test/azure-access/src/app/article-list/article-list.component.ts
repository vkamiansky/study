import { Component, OnInit } from '@angular/core';

import * as _ from 'underscore'
import 'msal'

import {
  Article
} from '../article';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit {

  articles: Article[];
  constructor() {
    this.articles = [
      {
        showDetailed: true,
        id: 1,
        heading: "Angular 4 In 4 Files",
        summary: "You don't need many files to get it working",
        text: "For the convenience of learning we are going to keep all components in one file. When building Angular apps the recommended approach is to have one component per file."
      },
      {
        showDetailed: true,
        id: 2,
        heading: "Compose Stuff",
        summary: "It's good to have page parts as components potentially reusable",
        text: "If you think of a typical webpage we can normally break it down into a set of logical components each with its own view, for example most webpages can be broken up into a header, footer and perhaps a sidebar."
      }
      ];
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
  }

}
