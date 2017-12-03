import { Injectable } from '@angular/core';
import { Article } from './article';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable()
export class ArticleService {

  private _articles: Article[];
  articles: BehaviorSubject<Article[]>;

  constructor() {
    this._articles = [
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
      this.articles = new BehaviorSubject<Article[]>([]);
  }

  updateArticles() : Promise<any> {
    return new Promise((resolve, reject) => {
      this.articles.next(this._articles);
      resolve();
    })
  }

  addArticle(article: Article) : Promise<any> {
    return new Promise((resolve, reject) => {
      if(-1 == article.id)
      {
        article.id = 1 + this._articles.reduce((a, x) => x.id > a ? x.id: a, -1);
      }
      this._articles.unshift(article);
      resolve();
    });
  }

  deleteArticle(article: Article) : Promise<any> {
    return new Promise((resolve, reject) => {
      let index = this._articles.findIndex((o) => o.id == article.id);
      if(-1 != index)
      {
        this._articles.splice(index, 1);
      }
      resolve();
    });
  }
}
