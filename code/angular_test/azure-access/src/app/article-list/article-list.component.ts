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

  updateArticlesList() {
    //this.articles = Observable.fromPromise(this.articleService.getArticles());
    let subj = new BehaviorSubject<Article[]>([]);
    this.articles = new Observable<Article[]>((observer: Subscriber<Article[]>) => {  
      subj.subscribe(x =>
        {
        let z = x;
      observer.next(z);
        }
      );      
      }
    );

    this.articles.subscribe(x => {
      console.log(x);
    });
    subj.next([{
      showDetailed: true,
      id: 1,
      heading: "Angular 4 In 4 Files",
      summary: "You don't need many files to get it working",
      text: "For the convenience of learning we are going to keep all components in one file. When building Angular apps the recommended approach is to have one component per file."
    }]);
  }
  
  addArticle(article: Article) {
    this.articleService.addArticle(article)
    .then(this.updateArticlesList);
  }

  deleteArticle(article: Article) {
    this.articleService.deleteArticle(article)
    .then(this.updateArticlesList);
  }

  ngOnInit() {
    this.updateArticlesList();
  }

}
